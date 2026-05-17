-- 1. EĞER VERİTABANI VARSA ÖNCE BAĞLANTILARINI KES VE SİL (Temiz kurulum için)
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'dbveritabani')
BEGIN
    ALTER DATABASE [dbveritabani] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [dbveritabani];
END
GO

-- 2. VERİTABANINI YENİDEN OLUŞTUR VE SEÇ
CREATE DATABASE [dbveritabani];
GO

USE [dbveritabani];
GO

-- 3. ANA TABLO 1: Kullanicilar (Kimseye bağımlı değil)
CREATE TABLE [dbo].[Kullanicilar] (
    [KullaniciID]           INT           IDENTITY (1, 1) NOT NULL,
    [Ad]                    NVARCHAR (50) NOT NULL,
    [Soyad]                 NVARCHAR (50) NOT NULL,
    [Cinsiyet]              BIT           NULL,
    [DogumTarihi]           DATE          NULL,
    [Boy]                   INT           NULL,
    [GuncelKilo]            FLOAT (53)    NULL,
    [HedefKilo]             FLOAT (53)    NULL,
    [KayitTarihi]           DATETIME      DEFAULT (getdate()) NULL,
    [VKI]                   FLOAT (53)    NULL,
    [BMH]                   FLOAT (53)    NULL,
    [GunlukKaloriIhtiyaci]  FLOAT (53)    NULL,
    [Yas]                   AS            (datediff(year,[DogumTarihi],getdate())),
    [AktiviteSeviyesi]      NVARCHAR (20) DEFAULT ('Hafif Hareketli') NULL,
    [AktiviteKatsayisi]     FLOAT (53)    DEFAULT ((1.3)) NULL,
    [SuBardak]              INT           DEFAULT ((0)) NOT NULL,
    [SuSonGuncellemeTarihi] DATE          DEFAULT (CONVERT([date],getdate())) NOT NULL,
    [Amac]                  NVARCHAR (50) DEFAULT ('Yag Yakmak / Kilo Vermek') NOT NULL,
    PRIMARY KEY CLUSTERED ([KullaniciID] ASC)
);
GO

-- 4. ANA TABLO 2: Besinler (Kimseye bağımlı değil)
CREATE TABLE [dbo].[Besinler] (
    [BesinID]      INT            IDENTITY (1, 1) NOT NULL,
    [BesinAdi]     NVARCHAR (100) NOT NULL,
    [Kalori]       FLOAT (53)     NOT NULL,
    [Protein]      FLOAT (53)     NULL,
    [Karbonhidrat] FLOAT (53)     NULL,
    [Yag]          FLOAT (53)     NULL,
    PRIMARY KEY CLUSTERED ([BesinID] ASC)
);
GO

-- 5. BAĞLI TABLO 1: Ogunler (Kullanicilar tablosuna bağlı)
CREATE TABLE [dbo].[Ogunler] (
    [OgunID]      INT           IDENTITY (1, 1) NOT NULL,
    [KullaniciID] INT           NOT NULL,
    [OgunAdi]     NVARCHAR (50) NULL,
    [Tarih]       DATETIME      DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([OgunID] ASC),
    FOREIGN KEY ([KullaniciID]) REFERENCES [dbo].[Kullanicilar] ([KullaniciID]) ON DELETE CASCADE
);
GO

-- 6. ARA/DETAY TABLO: OgunDetaylari (Hem Ogunler hem Besinler tablosuna bağlı)
CREATE TABLE [dbo].[OgunDetaylari] (
    [DetayID]    INT        IDENTITY (1, 1) NOT NULL,
    [OgunID]     INT        NOT NULL,
    [BesinID]    INT        NOT NULL,
    [MiktarGram] FLOAT (53) NOT NULL,
    PRIMARY KEY CLUSTERED ([DetayID] ASC),
    FOREIGN KEY ([OgunID]) REFERENCES [dbo].[Ogunler] ([OgunID]) ON DELETE CASCADE,
    FOREIGN KEY ([BesinID]) REFERENCES [dbo].[Besinler] ([BesinID]) ON DELETE CASCADE
);
GO