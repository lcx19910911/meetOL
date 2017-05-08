namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Speaker", "HeadImages", c => c.String(maxLength: 512, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Speaker", "HeadImages", c => c.String(maxLength: 32, unicode: false));
        }
    }
}
