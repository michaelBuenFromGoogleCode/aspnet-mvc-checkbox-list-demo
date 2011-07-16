using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using NHibernate;
using NHibernate.Dialect;

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Conventions.Helpers;

using AspNetMvcCheckboxList.Models;


namespace AspNetMvcCheckboxList
{
    public static class Mapper
    {
        static ISessionFactory _sf = null;
        public static ISessionFactory GetSessionFactory()
        {
            if (_sf != null) return _sf;

            var fc = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008.ConnectionString(@"Data Source=localhost;Initial Catalog=TestNhCheckboxList;User id=sa;Password=P@$$w0rd"))
                    .Mappings
                    (m =>
                            m.AutoMappings.Add
                            (
                                AutoMap.AssemblyOf<MvcApplication>(new CustomConfiguration())
                                   .Conventions.Add(ForeignKey.EndsWith("Id"))
                                   // .Conventions.Add(DynamicUpdate.AlwaysTrue())
                                   .Conventions.Add<RowversionConvention>()  
                                   .Override<Movie>(x => x.HasManyToMany(y => y.Genres).Table("MovieAssocGenre").ParentKeyColumn("MovieId").ChildKeyColumn("GenreId"))
                                   .Override<Genre>(x => x.HasManyToMany(y => y.Movies).Table("MovieAssocGenre").ParentKeyColumn("GenreId").ChildKeyColumn("MovieId"))                                  
                            )
                            
                    // .ExportTo(@"C:\_Misc\NH")                
                    );


            // Console.WriteLine( "{0}", string.Join( ";\n", fc.BuildConfiguration().GenerateSchemaCreationScript(new MsSql2008Dialect() ) ) );
            // Console.ReadLine();

            _sf = fc.BuildSessionFactory();
            return _sf;
        }


        class CustomConfiguration : DefaultAutomappingConfiguration
        {
            IList<Type> _objectsToMap = new List<Type>()
            {
                // whitelisted objects to map
                typeof(Movie), typeof(Genre)
            };
            public override bool ShouldMap(Type type) { return _objectsToMap.Any(x => x == type); }
            public override bool IsId(FluentNHibernate.Member member) { return member.Name == member.DeclaringType.Name + "Id"; }            
        }


        class RowversionConvention : IVersionConvention
        {
            public void Apply(IVersionInstance instance) { instance.Generated.Always(); }
        }


    }
}