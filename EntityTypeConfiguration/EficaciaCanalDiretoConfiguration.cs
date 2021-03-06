using Innovativo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Innovativo.EntityTypeConfiguration
{
    public class EficaciaCanalDiretoConfiguration : IEntityTypeConfiguration<EficaciaCanalDireto>
    {
        public void Configure(EntityTypeBuilder<EficaciaCanalDireto> builder)
        {
            builder.ToTable("eficaciacanaldireto").HasKey(ecd => ecd.ID);
            builder.ToTable("eficaciacanaldireto")
                    .HasOne(ecd => ecd.EficaciaCanaisRelatorio)
                    .WithOne(ecr => ecr.Direto);
        }
    }
}