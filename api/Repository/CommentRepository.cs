using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        public Task<List<Comment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}