using Microsoft.EntityFrameworkCore;
using TransactionsMVCApp.Models;

namespace TransactionsMVCApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<EventoTransaccion> EventosTransaccion { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaccion>()
                .HasMany(t => t.Eventos)
                .WithOne(e => e.Transaccion)
                .HasForeignKey(e => e.TransaccionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Notificacion)
                .WithOne(n => n.Transaccion)
                .HasForeignKey<Notificacion>(n => n.TransaccionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaccion>()
                .Property(t => t.Monto)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaccion>()
                .Property(t => t.TipoTransaccion)
                .HasMaxLength(50);

            modelBuilder.Entity<Transaccion>()
                .Property(t => t.CuentaDestino)
                .HasMaxLength(16)
                .IsFixedLength();

            modelBuilder.Entity<Transaccion>()
                .Property(t => t.DetallesAdicionales)
                .HasMaxLength(250);

            modelBuilder.Entity<EventoTransaccion>()
                .Property(e => e.TipoEvento)
                .IsRequired();

            modelBuilder.Entity<Transaccion>().HasData(
                new Transaccion
                {
                    TransaccionId = 1,
                    Monto = 120.50m,
                    TipoTransaccion = "TransferenciaBancaria",
                    CuentaDestino = "1234567890123456",
                    DetallesAdicionales = "Pago factura",
                    Estado = "Pendiente",
                    FechaCreacion = new DateTime(2024, 1, 1)
                },
                new Transaccion
                {
                    TransaccionId = 2,
                    Monto = 75.00m,
                    TipoTransaccion = "PagoConTarjeta",
                    CuentaDestino = "9999888877776666",
                    DetallesAdicionales = "Compra online",
                    Estado = "Exitosa",
                    FechaCreacion = new DateTime(2024, 1, 2),
                    FechaProcesamiento = new DateTime(2024, 1, 2)
                }
            );

            modelBuilder.Entity<EventoTransaccion>().HasData(
                new EventoTransaccion
                {
                    EventoId = 1,
                    TransaccionId = 1,
                    TipoEvento = "Creación",
                    Descripcion = "Transacción creada",
                    FechaEvento = new DateTime(2024, 1, 1)
                },
                new EventoTransaccion
                {
                    EventoId = 2,
                    TransaccionId = 2,
                    TipoEvento = "Procesamiento",
                    Descripcion = "Procesada correctamente",
                    FechaEvento = new DateTime(2024, 1, 2)
                }
            );

            modelBuilder.Entity<Notificacion>().HasData(
                new Notificacion
                {
                    NotificacionId = 1,
                    TransaccionId = 2,
                    EmailCliente = "cliente@test.com",
                    EstadoNotificacion = "Enviada",
                    FechaEnvio = new DateTime(2024, 1, 2)
                }
            );
        }
    }
}

