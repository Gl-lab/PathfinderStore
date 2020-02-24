using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Pathfinder.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Pathfinder.Infrastructure.Data
{
    public class PathfinderSeed
    {
        public static async Task SeedAsync(IServiceProvider app)
        {
            PgDbContext dbContext = app.GetRequiredService<PgDbContext>();
            if (!dbContext.ProductList.Any())
            {
                dbContext.ProductList.AddRange(GetPreconfiguredProduct());
                await dbContext.SaveChangesAsync();
            }
            if (!dbContext.DiceList.Any())
            {
                dbContext.DiceList.AddRange(GetPreconfiguredDice());
                await dbContext.SaveChangesAsync();
            }
            if (!dbContext.DamageTypeList.Any())
            {
                dbContext.DamageTypeList.AddRange(GetPreconfiguredDamageTypeList());
                await dbContext.SaveChangesAsync();
            }
            if (!dbContext.WeaponTypeList.Any())
            {
                dbContext.WeaponTypeList.AddRange(GetPreconfiguredWeaponTypeList());
                await dbContext.SaveChangesAsync();
            }
            
        }

        private static IEnumerable<WeaponType> GetPreconfiguredWeaponTypeList()
        {
            return new List<WeaponType>
            {
                new WeaponType { Name = "Безоружные атаки (Простое)", ShortName = "" }
                ,new WeaponType { Name = "Легкое оружие ближнего боя (Простое)", ShortName = "" }
                ,new WeaponType { Name = "Одноручное оружие ближнего боя (Простое)", ShortName = "" }
                ,new WeaponType { Name = "Оружие дальнего боя (Простое)", ShortName = "" }
                ,new WeaponType { Name = "Двуручное оружие ближнего боя (Простое)", ShortName = "" }
                ,new WeaponType { Name = "Легкое оружие ближнего боя (Воинское)", ShortName = "" }
                ,new WeaponType { Name = "Одноручное оружие ближнего боя (Воинское)", ShortName = "" }
                ,new WeaponType { Name = "Двуручное оружие ближнего боя (Воинское)", ShortName = "" }
                ,new WeaponType { Name = "Оружие дальнего боя (Воинское)", ShortName = "" }
                ,new WeaponType { Name = "Легкое оружие ближнего боя (Экзотическое)", ShortName = "" }
                ,new WeaponType { Name = "Одноручное оружие ближнего боя (Экзотическое)", ShortName = "" }
                ,new WeaponType { Name = "Двуручное оружие ближнего боя (Экзотическое)", ShortName = "" }
                ,new WeaponType { Name = "Оружие дальнего боя (Экзотическое)", ShortName = "" }
            };
        }
        private static IEnumerable<DamageType> GetPreconfiguredDamageTypeList()
        {
            return new List<DamageType>
            {
                new DamageType { Name = "Колющее", ShortName = "К"}
                ,new DamageType { Name = "Режущие", ShortName = "Р"}
                ,new DamageType { Name = "Дробящее", ShortName = "Д"}
            };
        }
        private static IEnumerable<BaseDice> GetPreconfiguredDice()
        {
            return new List<BaseDice>()
            {
                 new BaseDice {D = 2}
                ,new BaseDice {D = 3}
                ,new BaseDice {D = 4}
                ,new BaseDice {D = 6}
                ,new BaseDice {D = 8}
                ,new BaseDice {D = 10}
                ,new BaseDice {D = 12}
                ,new BaseDice {D = 20}
                ,new BaseDice {D = 100}
            };
        }

        private static IEnumerable<Product> GetPreconfiguredProduct()
        {
            Category FirstCategory = new Category { Name = "Оружие", Description = "" };
            Category SecondCategory = new Category { Name = "Доспех", Description = "" };
            return new List<Product>()
            {
                new Product()
                {
                    Name = "Штык"
                    ,Category = FirstCategory
                    ,Price = 5
                },
                new Product()
                {
                    Name = "Абордажная пика"
                    ,Category = FirstCategory
                    ,Price = 8
                },
                new Product()
                {
                    Name = "Кумадэ"
                    ,Category = FirstCategory
                    ,Price = 5
                },
                new Product()
                {
                    Name = "Длинное копье"
                    ,Category = FirstCategory
                    ,Price = 5
                },
                new Product()
                {
                    Name = "Дубина с железным наконечником"
                    ,Category = FirstCategory
                    ,Price = 0
                },
                new Product()
                {
                    Name = "Стеганая ткань"
                    ,Category = SecondCategory
                    ,Price = 100
                },
                new Product()
                {
                    Name = "Каменная дубина"
                    ,Category = FirstCategory
                    ,Price = 2
                },
                new Product()
                {
                    Name = "Тяжелая булава"
                    ,Category = FirstCategory
                    ,Price = 12
                },
                new Product()
                {
                    Name = "Моргенштерн "
                    ,Category = FirstCategory
                    ,Price = 8
                },
                new Product()
                {
                    Name = "Короткое копье"
                    ,Category = FirstCategory
                    ,Price = 1
                }
            };
        }
    }
}
