using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syndicate.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                username = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                password_hash = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "groups",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                invite_code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_groups", x => x.id);
                table.ForeignKey(
                    name: "fk_groups_users_owner_id",
                    column: x => x.owner_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "members",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                group_id = table.Column<Guid>(type: "uuid", nullable: false),
                role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_members", x => x.id);
                table.ForeignKey(
                    name: "fk_members_groups_group_id",
                    column: x => x.group_id,
                    principalTable: "groups",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_members_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "debts",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                group_id = table.Column<Guid>(type: "uuid", nullable: false),
                creditor_member_id = table.Column<Guid>(type: "uuid", nullable: false),
                debtor_member_id = table.Column<Guid>(type: "uuid", nullable: false),
                amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_debts", x => x.id);
                table.ForeignKey(
                    name: "fk_debts_groups_group_id",
                    column: x => x.group_id,
                    principalTable: "groups",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_debts_members_creditor_member_id",
                    column: x => x.creditor_member_id,
                    principalTable: "members",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_debts_members_debtor_member_id",
                    column: x => x.debtor_member_id,
                    principalTable: "members",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "polls",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                group_id = table.Column<Guid>(type: "uuid", nullable: false),
                creator_member_id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_polls", x => x.id);
                table.ForeignKey(
                    name: "fk_polls_groups_group_id",
                    column: x => x.group_id,
                    principalTable: "groups",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_polls_members_creator_member_id",
                    column: x => x.creator_member_id,
                    principalTable: "members",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "transactions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                member_id = table.Column<Guid>(type: "uuid", nullable: false),
                amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                debt_id = table.Column<Guid>(type: "uuid", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_transactions", x => x.id);
                table.ForeignKey(
                    name: "fk_transactions_debts_debt_id",
                    column: x => x.debt_id,
                    principalTable: "debts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "fk_transactions_members_member_id",
                    column: x => x.member_id,
                    principalTable: "members",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "poll_options",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                poll_id = table.Column<Guid>(type: "uuid", nullable: false),
                text = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_poll_options", x => x.id);
                table.ForeignKey(
                    name: "fk_poll_options_polls_poll_id",
                    column: x => x.poll_id,
                    principalTable: "polls",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "votes",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                poll_option_id = table.Column<Guid>(type: "uuid", nullable: false),
                member_id = table.Column<Guid>(type: "uuid", nullable: false),
                weight = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_votes", x => x.id);
                table.ForeignKey(
                    name: "fk_votes_members_member_id",
                    column: x => x.member_id,
                    principalTable: "members",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_votes_poll_options_poll_option_id",
                    column: x => x.poll_option_id,
                    principalTable: "poll_options",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_debts_creditor_member_id",
            table: "debts",
            column: "creditor_member_id");

        migrationBuilder.CreateIndex(
            name: "ix_debts_debtor_member_id",
            table: "debts",
            column: "debtor_member_id");

        migrationBuilder.CreateIndex(
            name: "ix_debts_group_id",
            table: "debts",
            column: "group_id");

        migrationBuilder.CreateIndex(
            name: "ix_groups_invite_code",
            table: "groups",
            column: "invite_code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_groups_owner_id",
            table: "groups",
            column: "owner_id");

        migrationBuilder.CreateIndex(
            name: "ix_members_group_id",
            table: "members",
            column: "group_id");

        migrationBuilder.CreateIndex(
            name: "ix_members_user_id_group_id",
            table: "members",
            columns: new[] { "user_id", "group_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_poll_options_poll_id",
            table: "poll_options",
            column: "poll_id");

        migrationBuilder.CreateIndex(
            name: "ix_polls_creator_member_id",
            table: "polls",
            column: "creator_member_id");

        migrationBuilder.CreateIndex(
            name: "ix_polls_group_id",
            table: "polls",
            column: "group_id");

        migrationBuilder.CreateIndex(
            name: "ix_transactions_debt_id",
            table: "transactions",
            column: "debt_id");

        migrationBuilder.CreateIndex(
            name: "ix_transactions_member_id",
            table: "transactions",
            column: "member_id");

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            table: "users",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_username",
            table: "users",
            column: "username",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_votes_member_id",
            table: "votes",
            column: "member_id");

        migrationBuilder.CreateIndex(
            name: "ix_votes_poll_option_id_member_id",
            table: "votes",
            columns: new[] { "poll_option_id", "member_id" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "transactions");

        migrationBuilder.DropTable(
            name: "votes");

        migrationBuilder.DropTable(
            name: "debts");

        migrationBuilder.DropTable(
            name: "poll_options");

        migrationBuilder.DropTable(
            name: "polls");

        migrationBuilder.DropTable(
            name: "members");

        migrationBuilder.DropTable(
            name: "groups");

        migrationBuilder.DropTable(
            name: "users");
    }
}
