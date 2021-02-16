using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace XAutomateMVC.Models.DBModels
{
    public partial class db_mateContext : DbContext
    {
        public db_mateContext()
        {
        }

        public db_mateContext(DbContextOptions<db_mateContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DbConfig> DbConfig { get; set; }
        public virtual DbSet<Exceute> Exceute { get; set; }
        public virtual DbSet<ExpectedResult> ExpectedResult { get; set; }
        public virtual DbSet<Login> Login { get; set; }
        public virtual DbSet<ReleaseNo> ReleaseNo { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RuleParamters> RuleParamters { get; set; }
        public virtual DbSet<Rules> Rules { get; set; }
        public virtual DbSet<Tablecolumn> Tablecolumn { get; set; }
        public virtual DbSet<TestApproach> TestApproach { get; set; }
        public virtual DbSet<TestCaseParameters> TestCaseParameters { get; set; }
        public virtual DbSet<TestCases> TestCases { get; set; }
        public virtual DbSet<TestSuite> TestSuite { get; set; }
        public virtual DbSet<TokenValue> TokenValue { get; set; }
        public virtual DbSet<WebFiles> WebFiles { get; set; }
        public virtual DbSet<WebTestcases> WebTestcases { get; set; }
        public virtual DbSet<Webtestcaselist> Webtestcaselist { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=52.178.152.165;port=3310;database=db_mate;user id=root;password=password;sslmode=none", x => x.ServerVersion("8.0.23-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbConfig>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DatabaseType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DbHostName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DbName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DbPassword)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DbPort)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.DbUser)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.SuiteName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Exceute>(entity =>
            {
                entity.HasKey(e => e.ExecuteId)
                    .HasName("PRIMARY");

                entity.Property(e => e.Execute).HasColumnType("datetime");

                entity.Property(e => e.Executiontime)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ResultUrl)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.SuiteName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Time)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<ExpectedResult>(entity =>
            {
                entity.HasKey(e => e.ExpectedResult1)
                    .HasName("PRIMARY");

                entity.Property(e => e.ExpectedResult1).HasColumnName("ExpectedResult");

                entity.Property(e => e.Expectedresultname)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Login>(entity =>
            {
                entity.HasIndex(e => e.RoleId)
                    .HasName("RolesId_idx");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EmailId)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EmployeeName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.EmployeeNo)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Password)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Role)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.UserName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.Login)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("RolesId");
            });

            modelBuilder.Entity<ReleaseNo>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReleaseName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ReleaseNo1)
                    .HasColumnName("ReleaseNo")
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Createdate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.RoleName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ScreenName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<RuleParamters>(entity =>
            {
                entity.HasKey(e => e.RuleParameterId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.RulesId)
                    .HasName("Rules_idx");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ParameterName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(10)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Rules)
                    .WithMany(p => p.RuleParamters)
                    .HasForeignKey(d => d.RulesId)
                    .HasConstraintName("Rules");
            });

            modelBuilder.Entity<Rules>(entity =>
            {
                entity.HasIndex(e => e.DbConfigId)
                    .HasName("DbConfigids_idx");

                entity.HasIndex(e => e.TestApproachid)
                    .HasName("TestApproachid_idx");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.RuleCondtion)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.RuleName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.RuleParameter)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TestApproachName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.DbConfig)
                    .WithMany(p => p.Rules)
                    .HasForeignKey(d => d.DbConfigId)
                    .HasConstraintName("DbConfigids");

                entity.HasOne(d => d.TestApproach)
                    .WithMany(p => p.Rules)
                    .HasForeignKey(d => d.TestApproachid)
                    .HasConstraintName("TestApproachid");
            });

            modelBuilder.Entity<Tablecolumn>(entity =>
            {
                entity.HasIndex(e => e.Dbconfigid)
                    .HasName("Dbconfig_idx");

                entity.Property(e => e.Active)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.FieldName)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Tablecolumn1)
                    .HasColumnName("Tablecolumn")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Dbconfig)
                    .WithMany(p => p.Tablecolumn)
                    .HasForeignKey(d => d.Dbconfigid)
                    .HasConstraintName("Dbconnection");
            });

            modelBuilder.Entity<TestApproach>(entity =>
            {
                entity.HasIndex(e => e.Dbconfigid)
                    .HasName("Dbconfigid_idx");

                entity.HasIndex(e => e.TestSuiteId)
                    .HasName("TestSuiteId_idx");

                entity.Property(e => e.Connectionname)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.SuiteIds)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TestApproachName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Dbconfig)
                    .WithMany(p => p.TestApproach)
                    .HasForeignKey(d => d.Dbconfigid)
                    .HasConstraintName("Dbconfigid");

                entity.HasOne(d => d.TestSuite)
                    .WithMany(p => p.TestApproach)
                    .HasForeignKey(d => d.TestSuiteId)
                    .HasConstraintName("TestSuiteId");
            });

            modelBuilder.Entity<TestCaseParameters>(entity =>
            {
                entity.HasIndex(e => e.TestCasesId)
                    .HasName("TestCasesId_idx");

                entity.Property(e => e.ParameterName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.TestCases)
                    .WithMany(p => p.TestCaseParameters)
                    .HasForeignKey(d => d.TestCasesId)
                    .HasConstraintName("TestCasesId");
            });

            modelBuilder.Entity<TestCases>(entity =>
            {
                entity.HasIndex(e => e.RulesId)
                    .HasName("RulesId_idx");

                entity.HasIndex(e => e.TestApproachid)
                    .HasName("TestApproachid_idx");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.ExceptedResult)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Expextedparameter)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TestCaseName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TestcaseTitle)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Testdata)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Rules)
                    .WithMany(p => p.TestCases)
                    .HasForeignKey(d => d.RulesId)
                    .HasConstraintName("RulesId");

                entity.HasOne(d => d.TestApproach)
                    .WithMany(p => p.TestCases)
                    .HasForeignKey(d => d.TestApproachid)
                    .HasConstraintName("TestCases_ibfk_1");
            });

            modelBuilder.Entity<TestSuite>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.TestSuitename)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<TokenValue>(entity =>
            {
                entity.HasKey(e => e.TokenId)
                    .HasName("PRIMARY");

                entity.Property(e => e.TockenId)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Validto).HasColumnType("datetime");
            });

            modelBuilder.Entity<WebFiles>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FileName)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FileType)
                    .HasColumnType("varchar(450)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.WebFilesPath)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<WebTestcases>(entity =>
            {
                entity.HasIndex(e => e.WebFilesId)
                    .HasName("WebFilesId_idx");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Testcase)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Webtestcase)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.WebFiles)
                    .WithMany(p => p.WebTestcases)
                    .HasForeignKey(d => d.WebFilesId)
                    .HasConstraintName("WebFilesId");
            });

            modelBuilder.Entity<Webtestcaselist>(entity =>
            {
                entity.HasIndex(e => e.WebTestcasesid)
                    .HasName("WebTestcasesid_idx");

                entity.Property(e => e.CaseType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Cases)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.WebTestcases)
                    .WithMany(p => p.Webtestcaselist)
                    .HasForeignKey(d => d.WebTestcasesid)
                    .HasConstraintName("WebTestcasesid");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
