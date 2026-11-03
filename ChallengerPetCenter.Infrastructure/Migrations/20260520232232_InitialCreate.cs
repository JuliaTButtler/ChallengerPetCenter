using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChallengerPetCenter.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "petdiary_users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TipoUsuario = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Ativo = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_petdiary_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "petdiary_pets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Especie = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Raca = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Observacoes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_petdiary_pets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_petdiary_pets_petdiary_users_UserId",
                        column: x => x.UserId,
                        principalTable: "petdiary_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "petdiary_veterinarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Crmv = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Especialidade = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_petdiary_veterinarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_petdiary_veterinarios_petdiary_users_UserId",
                        column: x => x.UserId,
                        principalTable: "petdiary_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "petdiary_diario_entradas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Data = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Resumo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    HumorGeral = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_petdiary_diario_entradas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_petdiary_diario_entradas_petdiary_pets_PetId",
                        column: x => x.PetId,
                        principalTable: "petdiary_pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "petdiary_pet_veterinario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DataInicio = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Ativo = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Observacoes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VeterinarioId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_petdiary_pet_veterinario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_petdiary_pet_veterinario_petdiary_pets_PetId",
                        column: x => x.PetId,
                        principalTable: "petdiary_pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_petdiary_pet_veterinario_petdiary_veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "petdiary_veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "petdiary_solicitacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Mensagem = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RespondidoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TutorId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VeterinarioId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_petdiary_solicitacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_petdiary_solicitacoes_petdiary_pets_PetId",
                        column: x => x.PetId,
                        principalTable: "petdiary_pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_petdiary_solicitacoes_petdiary_users_TutorId",
                        column: x => x.TutorId,
                        principalTable: "petdiary_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_petdiary_solicitacoes_petdiary_veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "petdiary_veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "petdiary_comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TextoComentario = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EditadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    DiarioEntradaId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UserId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_petdiary_comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_petdiary_comentarios_petdiary_diario_entradas_DiarioEntradaId",
                        column: x => x.DiarioEntradaId,
                        principalTable: "petdiary_diario_entradas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_petdiary_comentarios_petdiary_users_UserId",
                        column: x => x.UserId,
                        principalTable: "petdiary_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "petdiary_registros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Tipo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Subtipo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Valor = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    Unidade = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Nota = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Horario = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    AtualizadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    DiarioEntradaId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_petdiary_registros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_petdiary_registros_petdiary_diario_entradas_DiarioEntradaId",
                        column: x => x.DiarioEntradaId,
                        principalTable: "petdiary_diario_entradas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_comentarios_DiarioEntradaId",
                table: "petdiary_comentarios",
                column: "DiarioEntradaId");

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_comentarios_UserId",
                table: "petdiary_comentarios",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_diario_entradas_PetId_Data",
                table: "petdiary_diario_entradas",
                columns: new[] { "PetId", "Data" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_pet_veterinario_PetId",
                table: "petdiary_pet_veterinario",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_pet_veterinario_VeterinarioId",
                table: "petdiary_pet_veterinario",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_pets_UserId",
                table: "petdiary_pets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_registros_DiarioEntradaId",
                table: "petdiary_registros",
                column: "DiarioEntradaId");

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_solicitacoes_PetId",
                table: "petdiary_solicitacoes",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_solicitacoes_TutorId",
                table: "petdiary_solicitacoes",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_solicitacoes_VeterinarioId",
                table: "petdiary_solicitacoes",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_users_Email",
                table: "petdiary_users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_petdiary_veterinarios_UserId",
                table: "petdiary_veterinarios",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "petdiary_comentarios");

            migrationBuilder.DropTable(
                name: "petdiary_pet_veterinario");

            migrationBuilder.DropTable(
                name: "petdiary_registros");

            migrationBuilder.DropTable(
                name: "petdiary_solicitacoes");

            migrationBuilder.DropTable(
                name: "petdiary_diario_entradas");

            migrationBuilder.DropTable(
                name: "petdiary_veterinarios");

            migrationBuilder.DropTable(
                name: "petdiary_pets");

            migrationBuilder.DropTable(
                name: "petdiary_users");
        }
    }
}
