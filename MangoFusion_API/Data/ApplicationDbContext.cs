using MangoFusion_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MangoFusion_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Above we used Applciation USer which has few new properties along with all identity user
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {
            
        }
        /// <summary>
        /// adding new table for menuitems using DB set and migration
        /// </summary>
        public DbSet<MenuItem> MenuItems { get; set; }
        /// <summary>
        /// adding new table for Order Header which holde list of orders and pickup information details
        /// </summary>
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        /// <summary>
        /// Adding new table for holding order details liek order name and price and items selected
        /// </summary>
        public DbSet<OrderDetails> OrderDetails { get; set; }

        //Adding data in Menu Items as the above will create table, we need to pass data for pratice else we can also directly update the table.
        /// <summary>
        /// this will be protected override of On Model Creating class
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // this is default call for using the base options in DB context
            base.OnModelCreating(builder);
            builder.Entity<MenuItem>().HasData(
                new MenuItem
                {
                    id = 1,
                    name = "Spring Roll",
                    description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    image = "Images/spring roll.jpg",
                    price = 7.99,
                    category = "Appetizer",
                    specialTag = ""
                }, new MenuItem
                {
                    id = 2,
                    name = "Idli",
                    description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    image = "Images/idli.jpg",
                    price = 8.99,
                    category = "Appetizer",
                    specialTag = ""
                }, new MenuItem
                {
                    id = 3,
                    name = "Panu Puri",
                    description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    image = "Images/pani puri.jpg",
                    price = 8.99,
                    category = "Appetizer",
                    specialTag = "Best Seller"
                }, new MenuItem
                {
                    id = 4,
                    name = "Hakka Noodles",
                    description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    image = "Images/hakka noodles.jpg",
                    price = 10.99,
                    category = "Entrée",
                    specialTag = ""
                }, new MenuItem
                {
                    id = 5,
                    name = "Malai Kofta",
                    description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    image = "Images/malai kofta.jpg",
                    price = 12.99,
                    category = "Entrée",
                    specialTag = "Top Rated"
                }, new MenuItem
                {
                    id = 6,
                    name = "Paneer Pizza",
                    description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    image = "Images/paneer pizza.jpg",
                    price = 11.99,
                    category = "Entrée",
                    specialTag = ""
                }, new MenuItem
                {
                    id = 7,
                    name = "Paneer Tikka",
                    description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    image = "Images/paneer tikka.jpg",
                    price = 13.99,
                    category = "Entrée",
                    specialTag = "Chef's Special"
                }, new MenuItem
                {
                    id = 8,
                    name = "Carrot Love",
                    description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    image = "Images/carrot love.jpg",
                    price = 4.99,
                    category = "Dessert",
                    specialTag = ""
                }, new MenuItem
                {
                    id = 9,
                    name = "Rasmalai",
                    description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    image = "Images/rasmalai.jpg",
                    price = 4.99,
                    category = "Dessert",
                    specialTag = "Chef's Special"
                }, new MenuItem
                {
                    id = 10,
                    name = "Sweet Rolls",
                    description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    image = "Images/sweet rolls.jpg",
                    price = 3.99,
                    category = "Dessert",
                    specialTag = "Top Rated"
                });
        }
    }
}
