using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TransactionsMVCApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transacciones",
                columns: table => new
                {
                    TransaccionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipoTransaccion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CuentaDestino = table.Column<string>(type: "nchar(16)", fixedLength: true, maxLength: 16, nullable: false),
                    DetallesAdicionales = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaProcesamiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaNotificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacciones", x => x.TransaccionId);
                });

            migrationBuilder.CreateTable(
                name: "EventosTransaccion",
                columns: table => new
                {
                    EventoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransaccionId = table.Column<int>(type: "int", nullable: false),
                    TipoEvento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosTransaccion", x => x.EventoId);
                    table.ForeignKey(
                        name: "FK_EventosTransaccion_Transacciones_TransaccionId",
                        column: x => x.TransaccionId,
                        principalTable: "Transacciones",
                        principalColumn: "TransaccionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    NotificacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransaccionId = table.Column<int>(type: "int", nullable: false),
                    EmailCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoNotificacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.NotificacionId);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Transacciones_TransaccionId",
                        column: x => x.TransaccionId,
                        principalTable: "Transacciones",
                        principalColumn: "TransaccionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Transacciones",
                columns: new[] { "TransaccionId", "CuentaDestino", "DetallesAdicionales", "Estado", "FechaCreacion", "FechaNotificacion", "FechaProcesamiento", "Monto", "TipoTransaccion" },
                values: new object[,]
                {
                    { 1, "1234567890123456", "Pago factura", "Pendiente", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, 120.50m, "TransferenciaBancaria" },
                    { 2, "9999888877776666", "Compra online", "Exitosa", new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 75.00m, "PagoConTarjeta" }
                });

            migrationBuilder.InsertData(
                table: "EventosTransaccion",
                columns: new[] { "EventoId", "Descripcion", "FechaEvento", "TipoEvento", "TransaccionId" },
                values: new object[,]
                {
                    { 1, "Transacción creada", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Creación", 1 },
                    { 2, "Procesada correctamente", new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Procesamiento", 2 }
                });

            migrationBuilder.InsertData(
                table: "Notificaciones",
                columns: new[] { "NotificacionId", "EmailCliente", "EstadoNotificacion", "FechaEnvio", "TransaccionId" },
                values: new object[] { 1, "cliente@test.com", "Enviada", new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 });

            migrationBuilder.CreateIndex(
                name: "IX_EventosTransaccion_TransaccionId",
                table: "EventosTransaccion",
                column: "TransaccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_TransaccionId",
                table: "Notificaciones",
                column: "TransaccionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventosTransaccion");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "Transacciones");
        }
    }
}
