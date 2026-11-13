using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Chitai_Gorod.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ЕдиницыИзмерения> ЕдиницыИзмеренияs { get; set; }

    public virtual DbSet<Заказы> Заказыs { get; set; }

    public virtual DbSet<Индексы> Индексыs { get; set; }

    public virtual DbSet<Категории> Категорииs { get; set; }

    public virtual DbSet<НаселенныеПункты> НаселенныеПунктыs { get; set; }

    public virtual DbSet<Остатки> Остаткиs { get; set; }

    public virtual DbSet<ПозицииЗаказа> ПозицииЗаказаs { get; set; }

    public virtual DbSet<Пользователи> Пользователиs { get; set; }

    public virtual DbSet<Поставщики> Поставщикиs { get; set; }

    public virtual DbSet<Производители> Производителиs { get; set; }

    public virtual DbSet<ПунктыВыдачи> ПунктыВыдачиs { get; set; }

    public virtual DbSet<Роли> Ролиs { get; set; }

    public virtual DbSet<СтатусыЗаказа> СтатусыЗаказаs { get; set; }

    public virtual DbSet<Товары> Товарыs { get; set; }

    public virtual DbSet<Улицы> Улицыs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=chitai_gorod;Username=postgres;Password=postgres;SSL Mode=Disable");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ЕдиницыИзмерения>(entity =>
        {
            entity.HasKey(e => e.IdЕдиницыИзмерения).HasName("Единица_измерения_pkey");

            entity.ToTable("Единицы_измерения");

            entity.Property(e => e.IdЕдиницыИзмерения)
                .HasDefaultValueSql("nextval('\"Единица_измерен_ID_Единицы_измер_seq\"'::regclass)")
                .HasColumnName("ID_Единицы_измерения");
            entity.Property(e => e.Наименование).HasMaxLength(20);
        });

        modelBuilder.Entity<Заказы>(entity =>
        {
            entity.HasKey(e => e.IdЗаказа).HasName("Заказы_pkey");

            entity.ToTable("Заказы");

            entity.Property(e => e.IdЗаказа).HasColumnName("ID_Заказа");
            entity.Property(e => e.IdПользователя).HasColumnName("ID_Пользователя");
            entity.Property(e => e.IdПункта).HasColumnName("ID_Пункта");
            entity.Property(e => e.IdСтатусаЗаказа).HasColumnName("ID_Статуса_заказа");
            entity.Property(e => e.ДатаДоставки).HasColumnName("Дата доставки");
            entity.Property(e => e.ДатаЗаказа).HasColumnName("Дата заказа");
            entity.Property(e => e.КодДляПолучения).HasColumnName("Код для получения");
            entity.Property(e => e.НомерЗаказа).HasColumnName("Номер заказа");

            entity.HasOne(d => d.IdПользователяNavigation).WithMany(p => p.Заказыs)
                .HasForeignKey(d => d.IdПользователя)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_user");

            entity.HasOne(d => d.IdПунктаNavigation).WithMany(p => p.Заказыs)
                .HasForeignKey(d => d.IdПункта)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_pickpoint");

            entity.HasOne(d => d.IdСтатусаЗаказаNavigation).WithMany(p => p.Заказыs)
                .HasForeignKey(d => d.IdСтатусаЗаказа)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_status");
        });

        modelBuilder.Entity<Индексы>(entity =>
        {
            entity.HasKey(e => e.IdИндекса).HasName("Индекс_pkey");

            entity.ToTable("Индексы");

            entity.Property(e => e.IdИндекса)
                .HasDefaultValueSql("nextval('\"Индекс_ID_Индекса_seq\"'::regclass)")
                .HasColumnName("ID_Индекса");
            entity.Property(e => e.IdУлицы).HasColumnName("ID_Улицы");
            entity.Property(e => e.ПочтовыйИндекс).HasColumnName("Почтовый_индекс");

            entity.HasOne(d => d.IdУлицыNavigation).WithMany(p => p.Индексыs)
                .HasForeignKey(d => d.IdУлицы)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_index_street");
        });

        modelBuilder.Entity<Категории>(entity =>
        {
            entity.HasKey(e => e.IdКатегории).HasName("Категория_pkey");

            entity.ToTable("Категории");

            entity.Property(e => e.IdКатегории)
                .HasDefaultValueSql("nextval('\"Категория_ID_Категории_seq\"'::regclass)")
                .HasColumnName("ID_Категории");
            entity.Property(e => e.Наименование).HasMaxLength(50);
        });

        modelBuilder.Entity<НаселенныеПункты>(entity =>
        {
            entity.HasKey(e => e.IdНаселенногоПункта).HasName("Населенный_пункт_pkey");

            entity.ToTable("Населенные_пункты");

            entity.Property(e => e.IdНаселенногоПункта)
                .HasDefaultValueSql("nextval('\"Населенный_пунк_ID_Населенного_п_seq\"'::regclass)")
                .HasColumnName("ID_Населенного_пункта");
            entity.Property(e => e.Наименование).HasMaxLength(50);
        });

        modelBuilder.Entity<Остатки>(entity =>
        {
            entity.HasKey(e => e.IdОстатка).HasName("Остатки_pkey");

            entity.ToTable("Остатки");

            entity.Property(e => e.IdОстатка).HasColumnName("ID_Остатка");
            entity.Property(e => e.IdТовара)
                .HasMaxLength(20)
                .HasColumnName("ID_Товара");

            entity.HasOne(d => d.IdТовараNavigation).WithMany(p => p.Остаткиs)
                .HasForeignKey(d => d.IdТовара)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stock_product");
        });

        modelBuilder.Entity<ПозицииЗаказа>(entity =>
        {
            entity.HasKey(e => e.IdПозиции).HasName("Позиции_заказов_pkey");

            entity.ToTable("Позиции_заказа");

            entity.Property(e => e.IdПозиции)
                .HasDefaultValueSql("nextval('\"Позиции_заказов_ID_Позиции_seq\"'::regclass)")
                .HasColumnName("ID_Позиции");
            entity.Property(e => e.IdЗаказа).HasColumnName("ID_Заказа");
            entity.Property(e => e.IdТовара)
                .HasMaxLength(20)
                .HasColumnName("ID_Товара");

            entity.HasOne(d => d.IdЗаказаNavigation).WithMany(p => p.ПозицииЗаказаs)
                .HasForeignKey(d => d.IdЗаказа)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pos_order");

            entity.HasOne(d => d.IdТовараNavigation).WithMany(p => p.ПозицииЗаказаs)
                .HasForeignKey(d => d.IdТовара)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pos_product");
        });

        modelBuilder.Entity<Пользователи>(entity =>
        {
            entity.HasKey(e => e.IdПользователЯ).HasName("Пользователь_pkey");

            entity.ToTable("Пользователи");

            entity.Property(e => e.IdПользователЯ)
                .HasDefaultValueSql("nextval('\"Пользователь_ID_Пользователь_seq\"'::regclass)")
                .HasColumnName("ID_ПользователЯ");
            entity.Property(e => e.IdРоли).HasColumnName("ID_Роли");
            entity.Property(e => e.Имя).HasMaxLength(50);
            entity.Property(e => e.Логин).HasMaxLength(150);
            entity.Property(e => e.Отчество).HasMaxLength(50);
            entity.Property(e => e.Пароль).HasMaxLength(20);
            entity.Property(e => e.Фамилия).HasMaxLength(50);

            entity.HasOne(d => d.IdРолиNavigation).WithMany(p => p.Пользователиs)
                .HasForeignKey(d => d.IdРоли)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_role");
        });

        modelBuilder.Entity<Поставщики>(entity =>
        {
            entity.HasKey(e => e.IdПоставщика).HasName("Поставщик_pkey");

            entity.ToTable("Поставщики");

            entity.Property(e => e.IdПоставщика)
                .HasDefaultValueSql("nextval('\"Поставщик_ID_Поставщика_seq\"'::regclass)")
                .HasColumnName("ID_Поставщика");
            entity.Property(e => e.Наименование).HasMaxLength(50);
        });

        modelBuilder.Entity<Производители>(entity =>
        {
            entity.HasKey(e => e.IdПроизводителя).HasName("Производители_pkey");

            entity.ToTable("Производители");

            entity.Property(e => e.IdПроизводителя).HasColumnName("ID_Производителя");
            entity.Property(e => e.Наименование).HasMaxLength(50);
        });

        modelBuilder.Entity<ПунктыВыдачи>(entity =>
        {
            entity.HasKey(e => e.IdПункта).HasName("Пункты_выдачи_pkey");

            entity.ToTable("Пункты_выдачи");

            entity.Property(e => e.IdПункта).HasColumnName("ID_Пункта");
            entity.Property(e => e.IdИндекса).HasColumnName("ID_Индекса");
            entity.Property(e => e.НомерДома).HasColumnName("Номер_дома");

            entity.HasOne(d => d.IdИндексаNavigation).WithMany(p => p.ПунктыВыдачиs)
                .HasForeignKey(d => d.IdИндекса)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pickpoint_index");
        });

        modelBuilder.Entity<Роли>(entity =>
        {
            entity.HasKey(e => e.IdРоли).HasName("Роли_pkey");

            entity.ToTable("Роли");

            entity.Property(e => e.IdРоли).HasColumnName("ID_Роли");
            entity.Property(e => e.Наименование).HasMaxLength(50);
        });

        modelBuilder.Entity<СтатусыЗаказа>(entity =>
        {
            entity.HasKey(e => e.IdСтатусаЗаказа).HasName("Статусы_заказов_pkey");

            entity.ToTable("Статусы_заказа");

            entity.Property(e => e.IdСтатусаЗаказа)
                .HasDefaultValueSql("nextval('\"Статусы_заказов_ID_Статуса_заказ_seq\"'::regclass)")
                .HasColumnName("ID_Статуса_заказа");
            entity.Property(e => e.Наименование).HasMaxLength(20);
        });

        modelBuilder.Entity<Товары>(entity =>
        {
            entity.HasKey(e => e.IdТовара).HasName("Товары_pkey");

            entity.ToTable("Товары");

            entity.Property(e => e.IdТовара)
                .HasMaxLength(20)
                .HasColumnName("ID_Товара");
            entity.Property(e => e.IdЕдиницыИзмерения).HasColumnName("ID_Единицы_измерения");
            entity.Property(e => e.IdКатегории).HasColumnName("ID_Категории");
            entity.Property(e => e.IdПоставщика).HasColumnName("ID_Поставщика");
            entity.Property(e => e.IdПроизводителя).HasColumnName("ID_Производителя");
            entity.Property(e => e.Наименование).HasMaxLength(500);
            entity.Property(e => e.Описание).HasMaxLength(500);
            entity.Property(e => e.Фото).HasMaxLength(20);
            entity.Property(e => e.Цена).HasPrecision(15, 2);

            entity.HasOne(d => d.IdЕдиницыИзмеренияNavigation).WithMany(p => p.Товарыs)
                .HasForeignKey(d => d.IdЕдиницыИзмерения)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_unit");

            entity.HasOne(d => d.IdКатегорииNavigation).WithMany(p => p.Товарыs)
                .HasForeignKey(d => d.IdКатегории)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_category");

            entity.HasOne(d => d.IdПоставщикаNavigation).WithMany(p => p.Товарыs)
                .HasForeignKey(d => d.IdПоставщика)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_supplier");

            entity.HasOne(d => d.IdПроизводителяNavigation).WithMany(p => p.Товарыs)
                .HasForeignKey(d => d.IdПроизводителя)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_manufacturer");
        });

        modelBuilder.Entity<Улицы>(entity =>
        {
            entity.HasKey(e => e.IdУлицы).HasName("Улицы_pkey");

            entity.ToTable("Улицы");

            entity.Property(e => e.IdУлицы).HasColumnName("ID_Улицы");
            entity.Property(e => e.IdНаселенногоПункта).HasColumnName("ID_Населенного_пункта");
            entity.Property(e => e.Наименование).HasColumnType("character varying");

            entity.HasOne(d => d.IdНаселенногоПунктаNavigation).WithMany(p => p.Улицыs)
                .HasForeignKey(d => d.IdНаселенногоПункта)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_street_locality");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
