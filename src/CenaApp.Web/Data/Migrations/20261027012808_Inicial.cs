using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CenaApp.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alergenos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergenos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gustos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gustos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ingredientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Grupo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Sinonimos = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CuentaEnDespensa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restricciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restricciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Rol = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nivel = table.Column<string>(type: "text", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recetas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Emoji = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    TiempoMin = table.Column<int>(type: "integer", nullable: false),
                    Porciones = table.Column<int>(type: "integer", nullable: false),
                    Dificultad = table.Column<string>(type: "text", nullable: false),
                    Calorias = table.Column<int>(type: "integer", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recetas_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngredienteAlergenos",
                columns: table => new
                {
                    AlergenoId = table.Column<int>(type: "integer", nullable: false),
                    IngredienteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredienteAlergenos", x => new { x.AlergenoId, x.IngredienteId });
                    table.ForeignKey(
                        name: "FK_IngredienteAlergenos_Alergenos_AlergenoId",
                        column: x => x.AlergenoId,
                        principalTable: "Alergenos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredienteAlergenos_Ingredientes_IngredienteId",
                        column: x => x.IngredienteId,
                        principalTable: "Ingredientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngredienteRestriccionesIncompatibles",
                columns: table => new
                {
                    IngredienteId = table.Column<int>(type: "integer", nullable: false),
                    RestriccionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredienteRestriccionesIncompatibles", x => new { x.IngredienteId, x.RestriccionId });
                    table.ForeignKey(
                        name: "FK_IngredienteRestriccionesIncompatibles_Ingredientes_Ingredie~",
                        column: x => x.IngredienteId,
                        principalTable: "Ingredientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredienteRestriccionesIncompatibles_Restricciones_Restric~",
                        column: x => x.RestriccionId,
                        principalTable: "Restricciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Despensa",
                columns: table => new
                {
                    IngredienteId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Despensa", x => new { x.IngredienteId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_Despensa_Ingredientes_IngredienteId",
                        column: x => x.IngredienteId,
                        principalTable: "Ingredientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Despensa_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioAlergenos",
                columns: table => new
                {
                    AlergenoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioAlergenos", x => new { x.AlergenoId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_UsuarioAlergenos_Alergenos_AlergenoId",
                        column: x => x.AlergenoId,
                        principalTable: "Alergenos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioAlergenos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioGustos",
                columns: table => new
                {
                    GustoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioGustos", x => new { x.GustoId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_UsuarioGustos_Gustos_GustoId",
                        column: x => x.GustoId,
                        principalTable: "Gustos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioGustos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRestricciones",
                columns: table => new
                {
                    RestriccionId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRestricciones", x => new { x.RestriccionId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_UsuarioRestricciones_Restricciones_RestriccionId",
                        column: x => x.RestriccionId,
                        principalTable: "Restricciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRestricciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favoritos",
                columns: table => new
                {
                    RecetaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoritos", x => new { x.RecetaId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_Favoritos_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoritos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pasos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecetaId = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Texto = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pasos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pasos_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecetaGustos",
                columns: table => new
                {
                    GustoId = table.Column<int>(type: "integer", nullable: false),
                    RecetaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecetaGustos", x => new { x.GustoId, x.RecetaId });
                    table.ForeignKey(
                        name: "FK_RecetaGustos_Gustos_GustoId",
                        column: x => x.GustoId,
                        principalTable: "Gustos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecetaGustos_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecetaIngredientes",
                columns: table => new
                {
                    RecetaId = table.Column<int>(type: "integer", nullable: false),
                    IngredienteId = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    Unidad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Nota = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecetaIngredientes", x => new { x.RecetaId, x.IngredienteId });
                    table.ForeignKey(
                        name: "FK_RecetaIngredientes_Ingredientes_IngredienteId",
                        column: x => x.IngredienteId,
                        principalTable: "Ingredientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecetaIngredientes_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Despensa_UsuarioId",
                table: "Despensa",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_UsuarioId",
                table: "Favoritos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredienteAlergenos_IngredienteId",
                table: "IngredienteAlergenos",
                column: "IngredienteId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredienteRestriccionesIncompatibles_RestriccionId",
                table: "IngredienteRestriccionesIncompatibles",
                column: "RestriccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredientes_Nombre",
                table: "Ingredientes",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pasos_RecetaId",
                table: "Pasos",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_RecetaGustos_RecetaId",
                table: "RecetaGustos",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_RecetaIngredientes_IngredienteId",
                table: "RecetaIngredientes",
                column: "IngredienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_CategoriaId",
                table: "Recetas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_Nombre",
                table: "Recetas",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAlergenos_UsuarioId",
                table: "UsuarioAlergenos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioGustos_UsuarioId",
                table: "UsuarioGustos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRestricciones_UsuarioId",
                table: "UsuarioRestricciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Despensa");

            migrationBuilder.DropTable(
                name: "Favoritos");

            migrationBuilder.DropTable(
                name: "IngredienteAlergenos");

            migrationBuilder.DropTable(
                name: "IngredienteRestriccionesIncompatibles");

            migrationBuilder.DropTable(
                name: "Pasos");

            migrationBuilder.DropTable(
                name: "RecetaGustos");

            migrationBuilder.DropTable(
                name: "RecetaIngredientes");

            migrationBuilder.DropTable(
                name: "UsuarioAlergenos");

            migrationBuilder.DropTable(
                name: "UsuarioGustos");

            migrationBuilder.DropTable(
                name: "UsuarioRestricciones");

            migrationBuilder.DropTable(
                name: "Ingredientes");

            migrationBuilder.DropTable(
                name: "Recetas");

            migrationBuilder.DropTable(
                name: "Alergenos");

            migrationBuilder.DropTable(
                name: "Gustos");

            migrationBuilder.DropTable(
                name: "Restricciones");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
