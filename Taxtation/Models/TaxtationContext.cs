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
        public virtual DbSet<TxscountryDetail> TxscountryDetail { get; set; }
        public virtual DbSet<TxscurrencyDetail> TxscurrencyDetail { get; set; }
        public virtual DbSet<TxscustomerDetail> TxscustomerDetail { get; set; }
        public virtual DbSet<TxsitemDetail> TxsitemDetail { get; set; }
        public virtual DbSet<TxssiteDetail> TxssiteDetail { get; set; }
        public virtual DbSet<TxsstoreDetail> TxsstoreDetail { get; set; }
        public virtual DbSet<TxssupplierDetail> TxssupplierDetail { get; set; }
        public virtual DbSet<TxstaxDetail> TxstaxDetail { get; set; }
        public virtual DbSet<Txsuomdetail> Txsuomdetail { get; set; }
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

            modelBuilder.Entity<TxtpurchaseDetail>(entity =>
            {
                entity.HasKey(e => e.Sno);

                entity.ToTable("TXTPurchaseDetail");

                entity.Property(e => e.Sno).HasColumnName("SNo");

                entity.Property(e => e.ExciseId).HasColumnName("ExciseID");

                entity.Property(e => e.Id).HasMaxLength(450);

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
