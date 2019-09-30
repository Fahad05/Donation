using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Taxtation.Models
{
    public partial class TAXTATIONContext : DbContext
    {
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<TxsbankDetail> TxsbankDetail { get; set; }
        public virtual DbSet<TxscityDetail> TxscityDetail { get; set; }
        public virtual DbSet<Txscoadetail> Txscoadetail { get; set; }
        public virtual DbSet<TxscountryDetail> TxscountryDetail { get; set; }
        public virtual DbSet<TxscurrencyDetail> TxscurrencyDetail { get; set; }
        public virtual DbSet<TxscustomerDetail> TxscustomerDetail { get; set; }
        public virtual DbSet<TxsitemDetail> TxsitemDetail { get; set; }
        public virtual DbSet<TxsprojectDetail> TxsprojectDetail { get; set; }
        public virtual DbSet<TxssiteDetail> TxssiteDetail { get; set; }
        public virtual DbSet<TxsstoreDetail> TxsstoreDetail { get; set; }
        public virtual DbSet<TxssupplierDetail> TxssupplierDetail { get; set; }
        public virtual DbSet<TxstaxDetail> TxstaxDetail { get; set; }
        public virtual DbSet<Txsuomdetail> Txsuomdetail { get; set; }
        public virtual DbSet<TxtcreditNoteDetail> TxtcreditNoteDetail { get; set; }
        public virtual DbSet<TxtcreditNoteMaster> TxtcreditNoteMaster { get; set; }
        public virtual DbSet<TxtdebitNoteDetail> TxtdebitNoteDetail { get; set; }
        public virtual DbSet<TxtdebitNoteMaster> TxtdebitNoteMaster { get; set; }
        public virtual DbSet<TxtjournalMaster> TxtjournalMaster { get; set; }
        public virtual DbSet<Txtledger2> Txtledger2 { get; set; }
        public virtual DbSet<TxtledgerDetail> TxtledgerDetail { get; set; }
        public virtual DbSet<TxtopeningMaster> TxtopeningMaster { get; set; }
        public virtual DbSet<TxtpurchaseDetail> TxtpurchaseDetail { get; set; }
        public virtual DbSet<TxtpurchaseMaster> TxtpurchaseMaster { get; set; }
        public virtual DbSet<TxtsaleDetail> TxtsaleDetail { get; set; }
        public virtual DbSet<TxtsaleMaster> TxtsaleMaster { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Data Source=IT01;Initial Catalog=TAXTATION;Persist Security Info=True;User ID=sa;Password=Sirsyed45;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasIndex(e => e.UserId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            });

            modelBuilder.Entity<TxsbankDetail>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.BnkId });

                entity.ToTable("TXSBankDetail");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.BnkId).ValueGeneratedOnAdd();

                entity.Property(e => e.BnkAbbr).HasMaxLength(50);

                entity.Property(e => e.BnkAccNo).HasMaxLength(256);

                entity.Property(e => e.BnkBranch).HasMaxLength(256);

                entity.Property(e => e.BnkDesc).HasMaxLength(256);

                entity.Property(e => e.BnkEmail).HasMaxLength(256);

                entity.Property(e => e.BnkFax).HasMaxLength(256);

                entity.Property(e => e.BnkName).HasMaxLength(256);

                entity.Property(e => e.BnkPhNo).HasMaxLength(256);

                entity.Property(e => e.BnkType).HasMaxLength(50);

                entity.Property(e => e.BnkUrl)
                    .HasColumnName("BnkURL")
                    .HasMaxLength(256);

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<TxscityDetail>(entity =>
            {
                entity.HasKey(e => e.CtyCode);

                entity.ToTable("TXSCityDetail");

                entity.Property(e => e.CtyCode)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.CouCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CtyAbbr).HasMaxLength(50);

                entity.Property(e => e.CtyDesc).HasMaxLength(250);

                entity.Property(e => e.CtyName).HasMaxLength(250);

                entity.Property(e => e.EditBy).HasMaxLength(50);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(50);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.SttCode)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Txscoadetail>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.Coaid });

                entity.ToTable("TXSCOADetail");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.Coaid)
                    .HasColumnName("COAId")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AccAbbr).HasMaxLength(50);

                entity.Property(e => e.AccAccountNature).HasMaxLength(50);

                entity.Property(e => e.AccAccountSubNature).HasMaxLength(50);

                entity.Property(e => e.AccAccountType).HasMaxLength(50);

                entity.Property(e => e.AccAccountTypeShort).HasMaxLength(50);

                entity.Property(e => e.AccBsuCode).HasMaxLength(50);

                entity.Property(e => e.AccCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.AccComCode).HasMaxLength(50);

                entity.Property(e => e.AccDesc).HasMaxLength(250);

                entity.Property(e => e.AccEffectPeriodEnd).HasColumnType("smalldatetime");

                entity.Property(e => e.AccEffectPeriodStart).HasColumnType("smalldatetime");

                entity.Property(e => e.AccGrpCode).HasMaxLength(50);

                entity.Property(e => e.AccHierarchyCode).HasMaxLength(50);

                entity.Property(e => e.AccLevel).HasMaxLength(50);

                entity.Property(e => e.AccName).HasMaxLength(250);

                entity.Property(e => e.AccNameLevelWise).HasMaxLength(250);

                entity.Property(e => e.AccParentAccount).HasMaxLength(50);

                entity.Property(e => e.Bs).HasColumnName("BS");

                entity.Property(e => e.Ce).HasColumnName("CE");

                entity.Property(e => e.Ce1).HasColumnName("CE1");

                entity.Property(e => e.Ce2).HasColumnName("CE2");

                entity.Property(e => e.Cf).HasColumnName("CF");

                entity.Property(e => e.Cf1).HasColumnName("CF1");

                entity.Property(e => e.Cf2).HasColumnName("CF2");

                entity.Property(e => e.ClosingBalanceCr).HasColumnName("ClosingBalanceCR");

                entity.Property(e => e.EditBy).HasMaxLength(50);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EditOutSide).HasMaxLength(50);

                entity.Property(e => e.EnterBy).HasMaxLength(50);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.OpeningBalanceCr).HasColumnName("OpeningBalanceCR");

                entity.Property(e => e.Pl).HasColumnName("PL");

                entity.Property(e => e.TclosingBalance).HasColumnName("TClosingBalance");

                entity.Property(e => e.TclosingBalanceCr).HasColumnName("TClosingBalanceCR");

                entity.Property(e => e.TopeningBalance).HasColumnName("TOpeningBalance");

                entity.Property(e => e.TopeningBalanceCr).HasColumnName("TOpeningBalanceCR");

                entity.Property(e => e.TransactionCr).HasColumnName("TransactionCR");

                entity.Property(e => e.TransactionDr).HasColumnName("TransactionDR");

                entity.Property(e => e.TtransactionCr).HasColumnName("TTransactionCR");

                entity.Property(e => e.TtransactionDr).HasColumnName("TTransactionDR");
            });

            modelBuilder.Entity<TxscountryDetail>(entity =>
            {
                entity.HasKey(e => e.CouCode);

                entity.ToTable("TXSCountryDetail");

                entity.Property(e => e.CouCode)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.CouAbbr).HasMaxLength(50);

                entity.Property(e => e.CouDesc).HasMaxLength(250);

                entity.Property(e => e.CouGcc).HasColumnName("CouGCC");

                entity.Property(e => e.CouName).HasMaxLength(250);

                entity.Property(e => e.EditBy).HasMaxLength(50);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(50);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");
            });

            modelBuilder.Entity<TxscurrencyDetail>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.CurId });

                entity.ToTable("TXSCurrencyDetail");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.CurId).ValueGeneratedOnAdd();

                entity.Property(e => e.CurAbbr).HasMaxLength(50);

                entity.Property(e => e.CurName).HasMaxLength(256);

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<TxscustomerDetail>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.CusId });

                entity.ToTable("TXSCustomerDetail");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.CusId).ValueGeneratedOnAdd();

                entity.Property(e => e.CusAbbr).HasMaxLength(50);

                entity.Property(e => e.CusAddress).HasMaxLength(256);

                entity.Property(e => e.CusCity).HasMaxLength(256);

                entity.Property(e => e.CusCountry).HasMaxLength(256);

                entity.Property(e => e.CusDesc).HasMaxLength(256);

                entity.Property(e => e.CusEmail).HasMaxLength(256);

                entity.Property(e => e.CusFaxNo).HasMaxLength(256);

                entity.Property(e => e.CusName).HasMaxLength(256);

                entity.Property(e => e.CusNtn)
                    .HasColumnName("CusNTN")
                    .HasMaxLength(256);

                entity.Property(e => e.CusPerson).HasMaxLength(256);

                entity.Property(e => e.CusPhNo).HasMaxLength(256);

                entity.Property(e => e.CusStrn)
                    .HasColumnName("CusSTRN")
                    .HasMaxLength(256);

                entity.Property(e => e.CusType).HasMaxLength(256);

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<TxsitemDetail>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.ItmId });

                entity.ToTable("TXSItemDetail");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.ItmId)
                    .HasColumnName("ItmID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ItmBcode)
                    .HasColumnName("ItmBCode")
                    .HasMaxLength(256);

                entity.Property(e => e.ItmCp).HasColumnName("ItmCP");

                entity.Property(e => e.ItmName).HasMaxLength(256);

                entity.Property(e => e.ItmPid).HasColumnName("ItmPID");

                entity.Property(e => e.ItmSp).HasColumnName("ItmSP");

                entity.Property(e => e.ItmType).HasMaxLength(50);

                entity.Property(e => e.ItmUom)
                    .HasColumnName("ItmUOM")
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<TxsprojectDetail>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.ProId });

                entity.ToTable("TXSProjectDetail");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.ProId).ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ProAbbr).HasMaxLength(50);

                entity.Property(e => e.ProDesc).HasMaxLength(450);

                entity.Property(e => e.ProName).HasMaxLength(250);
            });

            modelBuilder.Entity<TxssiteDetail>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.SitId });

                entity.ToTable("TXSSiteDetail");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.SitId).ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.SitAbbr).HasMaxLength(50);

                entity.Property(e => e.SitDesc).HasMaxLength(450);

                entity.Property(e => e.SitName).HasMaxLength(250);
            });

            modelBuilder.Entity<TxsstoreDetail>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.StrId });

                entity.ToTable("TXSStoreDetail");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.StrId).ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.StrAbbr).HasMaxLength(50);

                entity.Property(e => e.StrDesc).HasMaxLength(256);

                entity.Property(e => e.StrName).HasMaxLength(256);
            });

            modelBuilder.Entity<TxssupplierDetail>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.SupId });

                entity.ToTable("TXSSupplierDetail");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.SupId).ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.SupAbbr).HasMaxLength(50);

                entity.Property(e => e.SupAddress).HasMaxLength(256);

                entity.Property(e => e.SupCity).HasMaxLength(256);

                entity.Property(e => e.SupCountry).HasMaxLength(256);

                entity.Property(e => e.SupDesc).HasMaxLength(256);

                entity.Property(e => e.SupEmail).HasMaxLength(256);

                entity.Property(e => e.SupFaxNo).HasMaxLength(256);

                entity.Property(e => e.SupName).HasMaxLength(256);

                entity.Property(e => e.SupNtn)
                    .HasColumnName("SupNTN")
                    .HasMaxLength(256);

                entity.Property(e => e.SupPerson).HasMaxLength(256);

                entity.Property(e => e.SupPhNo).HasMaxLength(256);

                entity.Property(e => e.SupStrn)
                    .HasColumnName("SupSTRN")
                    .HasMaxLength(256);

                entity.Property(e => e.SupType).HasMaxLength(256);
            });

            modelBuilder.Entity<TxstaxDetail>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.TaxId });

                entity.ToTable("TXSTaxDetail");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.TaxId).ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.TaxAbbr).HasMaxLength(50);

                entity.Property(e => e.TaxDesc).HasMaxLength(256);

                entity.Property(e => e.TaxName).HasMaxLength(256);

                entity.Property(e => e.TaxType).HasMaxLength(256);
            });

            modelBuilder.Entity<Txsuomdetail>(entity =>
            {
                entity.HasKey(e => e.Uomid);

                entity.ToTable("TXSUOMDetail");

                entity.Property(e => e.Uomid).HasColumnName("UOMId");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Uomabbr)
                    .HasColumnName("UOMAbbr")
                    .HasMaxLength(50);

                entity.Property(e => e.Uomactive).HasColumnName("UOMActive");

                entity.Property(e => e.Uomdesc)
                    .HasColumnName("UOMDesc")
                    .HasMaxLength(256);

                entity.Property(e => e.Uomname)
                    .HasColumnName("UOMName")
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<TxtcreditNoteDetail>(entity =>
            {
                entity.HasKey(e => e.Sno);

                entity.ToTable("TXTCreditNoteDetail");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ScnBatchNo).HasMaxLength(100);

                entity.Property(e => e.ScnCode).HasMaxLength(50);

                entity.Property(e => e.ScnExpiryDate).HasColumnType("smalldatetime");

                entity.Property(e => e.ScnSubRemarks).HasMaxLength(250);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<TxtcreditNoteMaster>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.ScnId });

                entity.ToTable("TXTCreditNoteMaster");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.ScnId).ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(50);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(50);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ScnCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ScnCondition).HasMaxLength(50);

                entity.Property(e => e.ScnDate).HasColumnType("smalldatetime");

                entity.Property(e => e.ScnExcPaidAmount).HasDefaultValueSql("((0))");

                entity.Property(e => e.ScnGpdate)
                    .HasColumnName("ScnGPDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.ScnGpno)
                    .HasColumnName("ScnGPNo")
                    .HasMaxLength(50);

                entity.Property(e => e.ScnInvoiceDate).HasColumnType("smalldatetime");

                entity.Property(e => e.ScnInvoiceNo).HasMaxLength(50);

                entity.Property(e => e.ScnLocType).HasMaxLength(50);

                entity.Property(e => e.ScnPostDate).HasColumnType("smalldatetime");

                entity.Property(e => e.ScnPostStatus).HasMaxLength(50);

                entity.Property(e => e.ScnRefNo).HasMaxLength(50);

                entity.Property(e => e.ScnRemarks).HasMaxLength(250);

                entity.Property(e => e.ScnSaleCode).HasMaxLength(50);

                entity.Property(e => e.ScnSaleDate).HasColumnType("smalldatetime");

                entity.Property(e => e.ScnSaleTrNo).HasMaxLength(50);

                entity.Property(e => e.ScnSaleType).HasMaxLength(50);

                entity.Property(e => e.ScnTotalBalance).HasDefaultValueSql("((0))");

                entity.Property(e => e.ScnTotalPaid).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TxtdebitNoteDetail>(entity =>
            {
                entity.HasKey(e => e.Sno);

                entity.ToTable("TXTDebitNoteDetail");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.PdnBatchNo).HasMaxLength(100);

                entity.Property(e => e.PdnCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PdnExpiryDate).HasColumnType("smalldatetime");

                entity.Property(e => e.PdnPoquantity).HasColumnName("PdnPOQuantity");

                entity.Property(e => e.PdnSubRemarks).HasMaxLength(250);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<TxtdebitNoteMaster>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.PdnId });

                entity.ToTable("TXTDebitNoteMaster");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.PdnId).ValueGeneratedOnAdd();

                entity.Property(e => e.AccId).HasMaxLength(50);

                entity.Property(e => e.EditBy).HasMaxLength(50);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(50);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.PdnBillNo).HasMaxLength(50);

                entity.Property(e => e.PdnCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PdnCondition).HasMaxLength(50);

                entity.Property(e => e.PdnDate).HasColumnType("smalldatetime");

                entity.Property(e => e.PdnGpdate)
                    .HasColumnName("PdnGPDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.PdnGpno)
                    .HasColumnName("PdnGPNo")
                    .HasMaxLength(50);

                entity.Property(e => e.PdnLocType).HasMaxLength(50);

                entity.Property(e => e.PdnPostDate).HasColumnType("smalldatetime");

                entity.Property(e => e.PdnPostStatus).HasMaxLength(50);

                entity.Property(e => e.PdnPurchaseCode).HasMaxLength(50);

                entity.Property(e => e.PdnPurchaseDate).HasColumnType("smalldatetime");

                entity.Property(e => e.PdnPurchaseTrNo).HasMaxLength(50);

                entity.Property(e => e.PdnPurchaseType).HasMaxLength(50);

                entity.Property(e => e.PdnRefNo).HasMaxLength(50);

                entity.Property(e => e.PdnRemarks).HasMaxLength(250);

                entity.Property(e => e.PdnTotalBalance).HasDefaultValueSql("((0))");

                entity.Property(e => e.PdnTotalPaid).HasDefaultValueSql("((0))");

                entity.Property(e => e.SitId).HasMaxLength(50);
            });

            modelBuilder.Entity<TxtjournalMaster>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.TrId });

                entity.ToTable("TXTJournalMaster");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.TrId).ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(50);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(50);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Tradjusting).HasColumnName("TRAdjusting");

                entity.Property(e => e.TrcomId).HasColumnName("TRComId");

                entity.Property(e => e.TrcurId).HasColumnName("TRCurId");

                entity.Property(e => e.Trdate)
                    .HasColumnName("TRDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrexchangeRate).HasColumnName("TRExchangeRate");

                entity.Property(e => e.Trgldate)
                    .HasColumnName("TRGLDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrgrpId).HasColumnName("TRGrpId");

                entity.Property(e => e.TrmainRemarks)
                    .HasColumnName("TRMainRemarks")
                    .HasMaxLength(250);

                entity.Property(e => e.Trno)
                    .IsRequired()
                    .HasColumnName("TRNo")
                    .HasMaxLength(50);

                entity.Property(e => e.TrpostDate)
                    .HasColumnName("TRPostDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrpostDateReverse)
                    .HasColumnName("TRPostDateReverse")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrpostStatus)
                    .HasColumnName("TRPostStatus")
                    .HasMaxLength(50);

                entity.Property(e => e.TrpostStatusReverse)
                    .HasColumnName("TRPostStatusReverse")
                    .HasMaxLength(50);

                entity.Property(e => e.TrrefMain)
                    .HasColumnName("TRRefMain")
                    .HasMaxLength(50);

                entity.Property(e => e.TrreverseDate)
                    .HasColumnName("TRReverseDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrreverseStatus).HasColumnName("TRReverseStatus");

                entity.Property(e => e.TrtotalAmount).HasColumnName("TRTotalAmount");
            });

            modelBuilder.Entity<Txtledger2>(entity =>
            {
                entity.HasKey(e => e.Sno);

                entity.ToTable("TXTLedger2");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.EditBy).HasMaxLength(50);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(50);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.TmpCustomer).HasMaxLength(250);

                entity.Property(e => e.TmpSupplier).HasMaxLength(250);

                entity.Property(e => e.TraccId).HasColumnName("TRAccId");

                entity.Property(e => e.TramountConverted).HasColumnName("TRAmountConverted");

                entity.Property(e => e.TrbarCode)
                    .HasColumnName("TRBarCode")
                    .HasMaxLength(50);

                entity.Property(e => e.TrchequeAccountNo)
                    .HasColumnName("TRChequeAccountNo")
                    .HasMaxLength(250);

                entity.Property(e => e.TrchequeBankName)
                    .HasColumnName("TRChequeBankName")
                    .HasMaxLength(250);

                entity.Property(e => e.TrchequeBranchName)
                    .HasColumnName("TRChequeBranchName")
                    .HasMaxLength(250);

                entity.Property(e => e.TrchequeDate)
                    .HasColumnName("TRChequeDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrchequeDepositDate)
                    .HasColumnName("TRChequeDepositDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrchequeNo)
                    .HasColumnName("TRChequeNo")
                    .HasMaxLength(250);

                entity.Property(e => e.Trcredit).HasColumnName("TRCredit");

                entity.Property(e => e.TrcurId).HasColumnName("TRCurId");

                entity.Property(e => e.Trdate)
                    .HasColumnName("TRDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.Trdebit).HasColumnName("TRDebit");

                entity.Property(e => e.TrdepositNo)
                    .HasColumnName("TRDepositNo")
                    .HasMaxLength(250);

                entity.Property(e => e.TrdocReference)
                    .HasColumnName("TRDocReference")
                    .HasMaxLength(250);

                entity.Property(e => e.TrentryType)
                    .HasColumnName("TREntryType")
                    .HasMaxLength(50);

                entity.Property(e => e.TrexchangeRate).HasColumnName("TRExchangeRate");

                entity.Property(e => e.Trgldate)
                    .HasColumnName("TRGLDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrinvAmount).HasColumnName("TRInvAmount");

                entity.Property(e => e.TrinvBalAmount).HasColumnName("TRInvBalAmount");

                entity.Property(e => e.TrinvDate)
                    .HasColumnName("TRInvDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrinvDisAmount).HasColumnName("TRInvDisAmount");

                entity.Property(e => e.TrinvId).HasColumnName("TRInvId");

                entity.Property(e => e.Trno)
                    .HasColumnName("TRNo")
                    .HasMaxLength(50);

                entity.Property(e => e.Trpddate)
                    .HasColumnName("TRPDDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrpostDate)
                    .HasColumnName("TRPostDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrpostStatus)
                    .HasColumnName("TRPostStatus")
                    .HasMaxLength(50);

                entity.Property(e => e.Trrecon).HasColumnName("TRRecon");

                entity.Property(e => e.TrrefAccId).HasColumnName("TRRefAccId");

                entity.Property(e => e.TrrefNo)
                    .HasColumnName("TRRefNo")
                    .HasMaxLength(250);

                entity.Property(e => e.Trremarks)
                    .HasColumnName("TRRemarks")
                    .HasMaxLength(250);

                entity.Property(e => e.TrserialNo).HasColumnName("TRSerialNo");

                entity.Property(e => e.Trstatus)
                    .HasColumnName("TRStatus")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'PENDING')");

                entity.Property(e => e.TrtaxAmount).HasColumnName("TRTaxAmount");

                entity.Property(e => e.TrtaxPercent).HasColumnName("TRTaxPercent");

                entity.Property(e => e.TrtxsId).HasColumnName("TRTxsId");

                entity.Property(e => e.Trtype)
                    .HasColumnName("TRType")
                    .HasMaxLength(50);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<TxtledgerDetail>(entity =>
            {
                entity.HasKey(e => e.Sno);

                entity.ToTable("TXTLedgerDetail");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.EditBy).HasMaxLength(50);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(50);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EntryFrom).HasMaxLength(50);

                entity.Property(e => e.EntryTypeDoc).HasMaxLength(50);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Pstdate)
                    .HasColumnName("PSTDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TmpCustomer).HasMaxLength(250);

                entity.Property(e => e.TmpSupplier).HasMaxLength(250);

                entity.Property(e => e.TraccId).HasColumnName("TRAccId");

                entity.Property(e => e.TragtAccId).HasColumnName("TRAgtAccId");

                entity.Property(e => e.TramountConverted).HasColumnName("TRAmountConverted");

                entity.Property(e => e.TrbsuId).HasColumnName("TRBsuId");

                entity.Property(e => e.TrchequeAccountNo)
                    .HasColumnName("TRChequeAccountNo")
                    .HasMaxLength(250);

                entity.Property(e => e.TrchequeBankName)
                    .HasColumnName("TRChequeBankName")
                    .HasMaxLength(250);

                entity.Property(e => e.TrchequeBranchName)
                    .HasColumnName("TRChequeBranchName")
                    .HasMaxLength(250);

                entity.Property(e => e.TrchequeDate)
                    .HasColumnName("TRChequeDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrchequeDepositDate)
                    .HasColumnName("TRChequeDepositDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrchequeMaturityDate)
                    .HasColumnName("TRChequeMaturityDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrchequeNo)
                    .HasColumnName("TRChequeNo")
                    .HasMaxLength(250);

                entity.Property(e => e.TrclosingBalance).HasColumnName("TRClosingBalance");

                entity.Property(e => e.TrcomId).HasColumnName("TRComId");

                entity.Property(e => e.TrconAccId).HasColumnName("TRConAccId");

                entity.Property(e => e.TrcosId).HasColumnName("TRCosId");

                entity.Property(e => e.Trcredit).HasColumnName("TRCredit");

                entity.Property(e => e.TrcurId).HasColumnName("TRCurId");

                entity.Property(e => e.TrcusAccId).HasColumnName("TRCusAccId");

                entity.Property(e => e.Trdate)
                    .HasColumnName("TRDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.Trdebit).HasColumnName("TRDebit");

                entity.Property(e => e.TrdepId).HasColumnName("TRDepId");

                entity.Property(e => e.TrdepositNo)
                    .HasColumnName("TRDepositNo")
                    .HasMaxLength(250);

                entity.Property(e => e.TrdisAccId).HasColumnName("TRDisAccId");

                entity.Property(e => e.TrdocRefBalanceAmount).HasColumnName("TRDocRefBalanceAmount");

                entity.Property(e => e.TrdocRefTotalAmount).HasColumnName("TRDocRefTotalAmount");

                entity.Property(e => e.TrdocReference)
                    .HasColumnName("TRDocReference")
                    .HasMaxLength(250);

                entity.Property(e => e.TrentryType)
                    .HasColumnName("TREntryType")
                    .HasMaxLength(50);

                entity.Property(e => e.TrexchangeRate).HasColumnName("TRExchangeRate");

                entity.Property(e => e.TrfucId).HasColumnName("TRFucId");

                entity.Property(e => e.Trgldate)
                    .HasColumnName("TRGLDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrgrpId).HasColumnName("TRGrpId");

                entity.Property(e => e.TrinvAmount).HasColumnName("TRInvAmount");

                entity.Property(e => e.TrinvBalAmount).HasColumnName("TRInvBalAmount");

                entity.Property(e => e.TrinvDate)
                    .HasColumnName("TRInvDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrinvDisAmount).HasColumnName("TRInvDisAmount");

                entity.Property(e => e.TrinvId).HasColumnName("TRInvId");

                entity.Property(e => e.TritmId).HasColumnName("TRItmId");

                entity.Property(e => e.TrivtAccId).HasColumnName("TRIvtAccId");

                entity.Property(e => e.Trno)
                    .HasColumnName("TRNo")
                    .HasMaxLength(50);

                entity.Property(e => e.TropeningBalance).HasColumnName("TROpeningBalance");

                entity.Property(e => e.Trpddate)
                    .HasColumnName("TRPDDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrpostDate)
                    .HasColumnName("TRPostDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrpostDateReverse)
                    .HasColumnName("TRPostDateReverse")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrpostStatus)
                    .HasColumnName("TRPostStatus")
                    .HasMaxLength(50);

                entity.Property(e => e.TrpostStatusReverse)
                    .HasColumnName("TRPostStatusReverse")
                    .HasMaxLength(50);

                entity.Property(e => e.TrproId).HasColumnName("TRProId");

                entity.Property(e => e.Trrecon).HasColumnName("TRRecon");

                entity.Property(e => e.TrrefAccId).HasColumnName("TRRefAccId");

                entity.Property(e => e.TrrefNo)
                    .HasColumnName("TRRefNo")
                    .HasMaxLength(250);

                entity.Property(e => e.Trremarks)
                    .HasColumnName("TRRemarks")
                    .HasMaxLength(250);

                entity.Property(e => e.TrreverseDate)
                    .HasColumnName("TRReverseDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrreverseStatus).HasColumnName("TRReverseStatus");

                entity.Property(e => e.TrsecId).HasColumnName("TRSecId");

                entity.Property(e => e.TrserialNo).HasColumnName("TRSerialNo");

                entity.Property(e => e.Trstatus)
                    .HasColumnName("TRStatus")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'PENDING')");

                entity.Property(e => e.TrsupAccId).HasColumnName("TRSupAccId");

                entity.Property(e => e.TrtaxAmount).HasColumnName("TRTaxAmount");

                entity.Property(e => e.TrtaxPercent).HasColumnName("TRTaxPercent");

                entity.Property(e => e.TrtxsId).HasColumnName("TRTxsId");

                entity.Property(e => e.Trtype)
                    .HasColumnName("TRType")
                    .HasMaxLength(50);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<TxtopeningMaster>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.TrId });

                entity.ToTable("TXTOpeningMaster");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.TrId).ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(50);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(50);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.SitCode).HasMaxLength(50);

                entity.Property(e => e.TrcomId).HasColumnName("TRComId");

                entity.Property(e => e.TrcurId).HasColumnName("TRCurId");

                entity.Property(e => e.Trdate)
                    .HasColumnName("TRDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrexchangeRate).HasColumnName("TRExchangeRate");

                entity.Property(e => e.Trgldate)
                    .HasColumnName("TRGLDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrgrpId).HasColumnName("TRGrpId");

                entity.Property(e => e.TrmainRemarks)
                    .HasColumnName("TRMainRemarks")
                    .HasMaxLength(250);

                entity.Property(e => e.Trno)
                    .IsRequired()
                    .HasColumnName("TRNo")
                    .HasMaxLength(50);

                entity.Property(e => e.TrpostDate)
                    .HasColumnName("TRPostDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrpostDateReverse)
                    .HasColumnName("TRPostDateReverse")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrpostStatus)
                    .HasColumnName("TRPostStatus")
                    .HasMaxLength(50);

                entity.Property(e => e.TrpostStatusReverse)
                    .HasColumnName("TRPostStatusReverse")
                    .HasMaxLength(50);

                entity.Property(e => e.TrrefMain)
                    .HasColumnName("TRRefMain")
                    .HasMaxLength(50);

                entity.Property(e => e.TrreverseDate)
                    .HasColumnName("TRReverseDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TrreverseStatus).HasColumnName("TRReverseStatus");

                entity.Property(e => e.TrtotalAmount).HasColumnName("TRTotalAmount");
            });

            modelBuilder.Entity<TxtpurchaseDetail>(entity =>
            {
                entity.HasKey(e => e.Sno);

                entity.ToTable("TXTPurchaseDetail");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.ExciseId).HasColumnName("ExciseID");

                entity.Property(e => e.Id).HasMaxLength(450);

                entity.Property(e => e.PurDisType).HasMaxLength(50);

                entity.Property(e => e.PurRemarks).HasMaxLength(500);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<TxtpurchaseMaster>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.PurId });

                entity.ToTable("TXTPurchaseMaster");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.PurId).ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.PurChqDate).HasColumnType("smalldatetime");

                entity.Property(e => e.PurChqNo).HasMaxLength(50);

                entity.Property(e => e.PurDate).HasColumnType("smalldatetime");

                entity.Property(e => e.PurDcref)
                    .HasColumnName("PurDCRef")
                    .HasMaxLength(50);

                entity.Property(e => e.PurItmType).HasMaxLength(50);

                entity.Property(e => e.PurPayTerm).HasMaxLength(50);

                entity.Property(e => e.PurPomapCode)
                    .HasColumnName("PurPOMapCode")
                    .HasMaxLength(50);

                entity.Property(e => e.PurPoref)
                    .HasColumnName("PurPORef")
                    .HasMaxLength(50);

                entity.Property(e => e.PurPrices).HasMaxLength(50);

                entity.Property(e => e.PurRemarks).HasMaxLength(450);

                entity.Property(e => e.PurScope).HasMaxLength(50);

                entity.Property(e => e.PurSupDate).HasColumnType("smalldatetime");

                entity.Property(e => e.PurType).HasMaxLength(50);
            });

            modelBuilder.Entity<TxtsaleDetail>(entity =>
            {
                entity.HasKey(e => e.Sno);

                entity.ToTable("TXTSaleDetail");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.Id).HasMaxLength(450);

                entity.Property(e => e.SalDisType).HasMaxLength(50);

                entity.Property(e => e.SalSubRemarks).HasMaxLength(500);

                entity.Property(e => e.SalVatper).HasColumnName("SalVATPer");

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<TxtsaleMaster>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.SalId });

                entity.ToTable("TXTSaleMaster");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.SalId).ValueGeneratedOnAdd();

                entity.Property(e => e.EditBy).HasMaxLength(256);

                entity.Property(e => e.EditDate).HasColumnType("smalldatetime");

                entity.Property(e => e.EnterBy).HasMaxLength(256);

                entity.Property(e => e.EnterDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.SalChqDate).HasColumnType("smalldatetime");

                entity.Property(e => e.SalChqNo).HasMaxLength(50);

                entity.Property(e => e.SalDate).HasColumnType("smalldatetime");

                entity.Property(e => e.SalItemType).HasMaxLength(50);

                entity.Property(e => e.SalLporef)
                    .HasColumnName("SalLPORef")
                    .HasMaxLength(50);

                entity.Property(e => e.SalPayTerms).HasMaxLength(50);

                entity.Property(e => e.SalPrices).HasMaxLength(50);

                entity.Property(e => e.SalRemarks).HasMaxLength(256);

                entity.Property(e => e.SalSalesMapCo).HasMaxLength(50);

                entity.Property(e => e.SalScope).HasMaxLength(50);

                entity.Property(e => e.SalSoRef).HasMaxLength(50);

                entity.Property(e => e.SalSupplyDate).HasColumnType("smalldatetime");

                entity.Property(e => e.SalType).HasMaxLength(50);
            });
        }
    }
}
