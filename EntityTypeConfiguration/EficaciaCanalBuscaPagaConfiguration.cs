using Innovativo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Innovativo.EntityTypeConfiguration
{
    public class EficaciaCanalBuscaPagaConfiguration : IEntityTypeConfiguration<EficaciaCanalBuscaPaga>
    {
        public void Configure(EntityTypeBuilder<EficaciaCanalBuscaPaga> builder)
        {
            builder.ToTable("eficaciacanalbuscapaga").HasKey(ecbp => ecbp.ID);
            builder.ToTable("eficaciacanalbuscapaga")
                    .HasOne(ecbp => ecbp.EficaciaCanaisRelatorio)
                    .WithOne(ecr => ecr.BuscaPaga);
        }
    }
}