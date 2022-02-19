using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups.Include(x => x.Connections)
                 .Where(x => x.Connections.Any(x => x.ConnectionId == connectionId))
                 .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.Include(m => m.Sender)
                .Include(m => m.Recipient)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(m => m.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.RecipientUserName == messageParams.Username
                && x.RecipientDeleted == false),
                "Outbox" => query.Where(x => x.SenderUserName == messageParams.Username
                && x.SenderDeleted == false),
                _ => query.Where(x => x.RecipientUserName == messageParams.Username
                && x.RecipientDeleted == false
                && x.DateRead == null),
            };

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserUsername,
            string recipientUsername)
        {
            var messages = await _context.Messages
            .Where(m => m.Recipient.UserName == currentUserUsername
            && m.RecipientDeleted == false
            && m.Sender.UserName == recipientUsername
            || m.Recipient.UserName == recipientUsername
            && m.Sender.UserName == currentUserUsername
            && m.SenderDeleted == false)
            .OrderBy(m => m.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null
            && m.RecipientUserName == currentUserUsername).ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

            }
            return messages;
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }


    }
}