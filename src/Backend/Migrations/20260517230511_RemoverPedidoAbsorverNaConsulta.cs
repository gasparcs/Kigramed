using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoverPedidoAbsorverNaConsulta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb01_perfil",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb01_perfil", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb03_tipo_contacto",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb03_tipo_contacto", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb06_especialidade",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb06_especialidade", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb09_cliente",
                columns: table => new
                {
                    nif = table.Column<string>(type: "text", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb09_cliente", x => x.nif);
                });

            migrationBuilder.CreateTable(
                name: "tb10_genero",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb10_genero", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb11_cliente_paciente",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb11_cliente_paciente", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb13_estado_consulta",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb13_estado_consulta", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb16_permissoes",
                columns: table => new
                {
                    uuid_permissoes = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb16_permissoes", x => x.uuid_permissoes);
                });

            migrationBuilder.CreateTable(
                name: "tb02_funcionario",
                columns: table => new
                {
                    nif = table.Column<string>(type: "text", nullable: false),
                    id_perfil = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb02_funcionario", x => x.nif);
                    table.ForeignKey(
                        name: "FK_tb02_funcionario_tb01_perfil_id_perfil",
                        column: x => x.id_perfil,
                        principalTable: "tb01_perfil",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb08_servico",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_especialidade = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    duracao_minuto = table.Column<int>(type: "integer", nullable: false),
                    preco = table.Column<decimal>(type: "numeric", nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb08_servico", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb08_servico_tb06_especialidade_id_especialidade",
                        column: x => x.id_especialidade,
                        principalTable: "tb06_especialidade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb12_paciente",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nif_cliente = table.Column<string>(type: "text", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    data_nascimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    id_genero = table.Column<int>(type: "integer", nullable: false),
                    id_cliente_paciente = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb12_paciente", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb12_paciente_tb09_cliente_nif_cliente",
                        column: x => x.nif_cliente,
                        principalTable: "tb09_cliente",
                        principalColumn: "nif",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb12_paciente_tb10_genero_id_genero",
                        column: x => x.id_genero,
                        principalTable: "tb10_genero",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb12_paciente_tb11_cliente_paciente_id_cliente_paciente",
                        column: x => x.id_cliente_paciente,
                        principalTable: "tb11_cliente_paciente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb17_perfil_permissoes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid_permissoes = table.Column<Guid>(type: "uuid", nullable: false),
                    id_perfil = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb17_perfil_permissoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb17_perfil_permissoes_tb01_perfil_id_perfil",
                        column: x => x.id_perfil,
                        principalTable: "tb01_perfil",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb17_perfil_permissoes_tb16_permissoes_uuid_permissoes",
                        column: x => x.uuid_permissoes,
                        principalTable: "tb16_permissoes",
                        principalColumn: "uuid_permissoes",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb04_contacto",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nif_funcionario = table.Column<string>(type: "text", nullable: true),
                    id_tipo_contacto = table.Column<int>(type: "integer", nullable: false),
                    id_cliente = table.Column<string>(type: "text", nullable: true),
                    contacto = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb04_contacto", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb04_contacto_tb02_funcionario_nif_funcionario",
                        column: x => x.nif_funcionario,
                        principalTable: "tb02_funcionario",
                        principalColumn: "nif");
                    table.ForeignKey(
                        name: "FK_tb04_contacto_tb03_tipo_contacto_id_tipo_contacto",
                        column: x => x.id_tipo_contacto,
                        principalTable: "tb03_tipo_contacto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb04_contacto_tb09_cliente_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "tb09_cliente",
                        principalColumn: "nif");
                });

            migrationBuilder.CreateTable(
                name: "tb05_auth",
                columns: table => new
                {
                    nif_funcionario = table.Column<string>(type: "text", nullable: false),
                    senha_hash = table.Column<string>(type: "text", nullable: false),
                    senha_salt = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb05_auth", x => x.nif_funcionario);
                    table.ForeignKey(
                        name: "FK_tb05_auth_tb02_funcionario_nif_funcionario",
                        column: x => x.nif_funcionario,
                        principalTable: "tb02_funcionario",
                        principalColumn: "nif",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb07_medico_especialidade",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nif_funcionario = table.Column<string>(type: "text", nullable: false),
                    id_especialidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb07_medico_especialidade", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb07_medico_especialidade_tb02_funcionario_nif_funcionario",
                        column: x => x.nif_funcionario,
                        principalTable: "tb02_funcionario",
                        principalColumn: "nif",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb07_medico_especialidade_tb06_especialidade_id_especialida~",
                        column: x => x.id_especialidade,
                        principalTable: "tb06_especialidade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb14_pagamento",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_cliente = table.Column<string>(type: "text", nullable: false),
                    id_secretaria = table.Column<string>(type: "text", nullable: false),
                    comprovativo = table.Column<string>(type: "text", nullable: false),
                    data_envio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    caminho_comprovativo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb14_pagamento", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb14_pagamento_tb02_funcionario_id_secretaria",
                        column: x => x.id_secretaria,
                        principalTable: "tb02_funcionario",
                        principalColumn: "nif",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb14_pagamento_tb09_cliente_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "tb09_cliente",
                        principalColumn: "nif",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb20_sms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_envio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false),
                    nif_funcionario = table.Column<string>(type: "text", nullable: false),
                    id_cliente = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb20_sms", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb20_sms_tb02_funcionario_nif_funcionario",
                        column: x => x.nif_funcionario,
                        principalTable: "tb02_funcionario",
                        principalColumn: "nif",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb20_sms_tb09_cliente_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "tb09_cliente",
                        principalColumn: "nif",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb15_consulta",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_medico_especialista = table.Column<int>(type: "integer", nullable: false),
                    id_servico = table.Column<int>(type: "integer", nullable: false),
                    id_paciente = table.Column<int>(type: "integer", nullable: true),
                    id_estado = table.Column<int>(type: "integer", nullable: false),
                    data_consulta = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    numero_pedido = table.Column<string>(type: "text", nullable: false),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    prazo_pagamento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb15_consulta", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb15_consulta_tb07_medico_especialidade_id_medico_especiali~",
                        column: x => x.id_medico_especialista,
                        principalTable: "tb07_medico_especialidade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb15_consulta_tb08_servico_id_servico",
                        column: x => x.id_servico,
                        principalTable: "tb08_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb15_consulta_tb12_paciente_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "tb12_paciente",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tb15_consulta_tb13_estado_consulta_id_estado",
                        column: x => x.id_estado,
                        principalTable: "tb13_estado_consulta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb18_pagamento_consulta",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_pagamento = table.Column<int>(type: "integer", nullable: false),
                    id_consulta = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb18_pagamento_consulta", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb18_pagamento_consulta_tb14_pagamento_id_pagamento",
                        column: x => x.id_pagamento,
                        principalTable: "tb14_pagamento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb18_pagamento_consulta_tb15_consulta_id_consulta",
                        column: x => x.id_consulta,
                        principalTable: "tb15_consulta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb19_medico_consulta",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_medico_especialidade = table.Column<int>(type: "integer", nullable: false),
                    id_consulta = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb19_medico_consulta", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb19_medico_consulta_tb07_medico_especialidade_id_medico_es~",
                        column: x => x.id_medico_especialidade,
                        principalTable: "tb07_medico_especialidade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb19_medico_consulta_tb15_consulta_id_consulta",
                        column: x => x.id_consulta,
                        principalTable: "tb15_consulta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tb13_estado_consulta",
                columns: new[] { "id", "descricao" },
                values: new object[,]
                {
                    { 1, "Pendente" },
                    { 2, "Aguarda Pagamento" },
                    { 3, "Comprovativo Enviado" },
                    { 4, "Confirmada" },
                    { 5, "Cancelada" },
                    { 6, "Finalizada" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb02_funcionario_id_perfil",
                table: "tb02_funcionario",
                column: "id_perfil");

            migrationBuilder.CreateIndex(
                name: "IX_tb04_contacto_id_cliente",
                table: "tb04_contacto",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_tb04_contacto_id_tipo_contacto",
                table: "tb04_contacto",
                column: "id_tipo_contacto");

            migrationBuilder.CreateIndex(
                name: "IX_tb04_contacto_nif_funcionario",
                table: "tb04_contacto",
                column: "nif_funcionario");

            migrationBuilder.CreateIndex(
                name: "IX_tb07_medico_especialidade_id_especialidade",
                table: "tb07_medico_especialidade",
                column: "id_especialidade");

            migrationBuilder.CreateIndex(
                name: "IX_tb07_medico_especialidade_nif_funcionario",
                table: "tb07_medico_especialidade",
                column: "nif_funcionario");

            migrationBuilder.CreateIndex(
                name: "IX_tb08_servico_id_especialidade",
                table: "tb08_servico",
                column: "id_especialidade");

            migrationBuilder.CreateIndex(
                name: "IX_tb12_paciente_id_cliente_paciente",
                table: "tb12_paciente",
                column: "id_cliente_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_tb12_paciente_id_genero",
                table: "tb12_paciente",
                column: "id_genero");

            migrationBuilder.CreateIndex(
                name: "IX_tb12_paciente_nif_cliente",
                table: "tb12_paciente",
                column: "nif_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_tb14_pagamento_id_cliente",
                table: "tb14_pagamento",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_tb14_pagamento_id_secretaria",
                table: "tb14_pagamento",
                column: "id_secretaria");

            migrationBuilder.CreateIndex(
                name: "IX_tb15_consulta_id_estado",
                table: "tb15_consulta",
                column: "id_estado");

            migrationBuilder.CreateIndex(
                name: "IX_tb15_consulta_id_medico_especialista",
                table: "tb15_consulta",
                column: "id_medico_especialista");

            migrationBuilder.CreateIndex(
                name: "IX_tb15_consulta_id_paciente",
                table: "tb15_consulta",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_tb15_consulta_id_servico",
                table: "tb15_consulta",
                column: "id_servico");

            migrationBuilder.CreateIndex(
                name: "IX_tb17_perfil_permissoes_id_perfil",
                table: "tb17_perfil_permissoes",
                column: "id_perfil");

            migrationBuilder.CreateIndex(
                name: "IX_tb17_perfil_permissoes_uuid_permissoes",
                table: "tb17_perfil_permissoes",
                column: "uuid_permissoes");

            migrationBuilder.CreateIndex(
                name: "IX_tb18_pagamento_consulta_id_consulta",
                table: "tb18_pagamento_consulta",
                column: "id_consulta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb18_pagamento_consulta_id_pagamento",
                table: "tb18_pagamento_consulta",
                column: "id_pagamento");

            migrationBuilder.CreateIndex(
                name: "IX_tb19_medico_consulta_id_consulta",
                table: "tb19_medico_consulta",
                column: "id_consulta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb19_medico_consulta_id_medico_especialidade",
                table: "tb19_medico_consulta",
                column: "id_medico_especialidade");

            migrationBuilder.CreateIndex(
                name: "IX_tb20_sms_id_cliente",
                table: "tb20_sms",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_tb20_sms_nif_funcionario",
                table: "tb20_sms",
                column: "nif_funcionario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb04_contacto");

            migrationBuilder.DropTable(
                name: "tb05_auth");

            migrationBuilder.DropTable(
                name: "tb17_perfil_permissoes");

            migrationBuilder.DropTable(
                name: "tb18_pagamento_consulta");

            migrationBuilder.DropTable(
                name: "tb19_medico_consulta");

            migrationBuilder.DropTable(
                name: "tb20_sms");

            migrationBuilder.DropTable(
                name: "tb03_tipo_contacto");

            migrationBuilder.DropTable(
                name: "tb16_permissoes");

            migrationBuilder.DropTable(
                name: "tb14_pagamento");

            migrationBuilder.DropTable(
                name: "tb15_consulta");

            migrationBuilder.DropTable(
                name: "tb07_medico_especialidade");

            migrationBuilder.DropTable(
                name: "tb08_servico");

            migrationBuilder.DropTable(
                name: "tb12_paciente");

            migrationBuilder.DropTable(
                name: "tb13_estado_consulta");

            migrationBuilder.DropTable(
                name: "tb02_funcionario");

            migrationBuilder.DropTable(
                name: "tb06_especialidade");

            migrationBuilder.DropTable(
                name: "tb09_cliente");

            migrationBuilder.DropTable(
                name: "tb10_genero");

            migrationBuilder.DropTable(
                name: "tb11_cliente_paciente");

            migrationBuilder.DropTable(
                name: "tb01_perfil");
        }
    }
}
