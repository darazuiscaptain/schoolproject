﻿using ExtraSjaj.Common.Interfaces;
using ExtraSjaj.Common.Models;
using ExtraSjaj.DAL.Context;

namespace ExtraSjaj.DAL.RepoPattern
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        protected readonly ExtraSjajContext _context;

        public UserRepository(ExtraSjajContext context) : base(context)
        {
            _context = context;
        }

        public ExtraSjajContext context
        {
            get { return context as ExtraSjajContext; }
        }
    }
}