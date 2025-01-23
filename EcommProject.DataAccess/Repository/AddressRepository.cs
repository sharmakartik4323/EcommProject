﻿using EcommProject.DataAccess.Data;
using EcommProject.DataAccess.Repository.IRepository;
using EcommProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject.DataAccess.Repository
{
    public class AddressRepository:Repository<Address>,IAddressRepository
    {
        private readonly ApplicationDbContext _context;
        public AddressRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}