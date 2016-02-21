namespace TransactionBelk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionStatus",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        code = c.Int(nullable: false),
                        label = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        createdTimestamp = c.DateTime(nullable: false),
                        sender = c.String(),
                        receiver = c.String(),
                        bankNumberSender = c.String(),
                        emailSender = c.String(),
                        phoneSender = c.String(),
                        approvalCertificate = c.Binary(),
                        transStatusId = c.Int(),
                        approvalTimestamp = c.DateTime(),
                        isDeleted = c.Boolean(nullable: false),
                        approver_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Users", t => t.approver_id)
                .ForeignKey("dbo.TransactionStatus", t => t.transStatusId)
                .Index(t => t.transStatusId)
                .Index(t => t.approver_id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        login = c.String(),
                        password = c.String(),
                        permission = c.Int(nullable: false),
                        createdTimestamp = c.DateTime(nullable: false),
                        isDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "TransactionStatusId", "dbo.TransactionStatus");
            DropForeignKey("dbo.Transactions", "approver_id", "dbo.Users");
            DropIndex("dbo.Transactions", new[] { "approver_id" });
            DropIndex("dbo.Transactions", new[] { "TransactionStatusId" });
            DropTable("dbo.Users");
            DropTable("dbo.Transactions");
            DropTable("dbo.TransactionStatus");
        }
    }
}
