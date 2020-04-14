using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CMSOnlineStore.Models.Data
{
    public class Db : DbContext
    {
        //Урок 3
        public DbSet<PagesDTO> Pages { get; set; }

        //Урок 6
        public DbSet<SidebarDTO> Sidebars { get; set; }

        //Урок 8
        public DbSet<CategoryDTO> Categories { get; set; }

        //Урок 11
        public DbSet<ProductDTO> Products { get; set; }

        // Урок 22
        public DbSet<UserDTO> Users { get; set; }

        // Урок 22
        public DbSet<RoleDTO> Roles { get; set; }

        // Урок 23
        public DbSet<UserRoleDTO> UserRoles { get; set; }

        // Урок 25
        public DbSet<OrderDTO> Orders { get; set; }
        public DbSet<OrderDetailsDTO> OrderDetails { get; set; }

    }
}