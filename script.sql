Build started...
Build succeeded.
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Usuario] (
    [UsuarioId] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    [EmailPrincipal] nvarchar(max) NOT NULL,
    [Senha] nvarchar(max) NOT NULL,
    [Telefone] nvarchar(max) NOT NULL,
    [Logradouro] nvarchar(max) NOT NULL,
    [Numero] nvarchar(max) NOT NULL,
    [Complemento] nvarchar(max) NOT NULL,
    [Bairro] nvarchar(max) NOT NULL,
    [Cidade] nvarchar(max) NOT NULL,
    [Estado] nvarchar(max) NOT NULL,
    [Cep] nvarchar(max) NOT NULL,
    [Tipo] nvarchar(max) NOT NULL,
    [Latitude] float NULL,
    [Longitude] float NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY ([UsuarioId])
);

CREATE TABLE [Contato] (
    [ContatoId] int NOT NULL IDENTITY,
    [Tipo] nvarchar(max) NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    [Valor] nvarchar(max) NOT NULL,
    [UsuarioId] int NOT NULL,
    CONSTRAINT [PK_Contato] PRIMARY KEY ([ContatoId]),
    CONSTRAINT [FK_Contato_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([UsuarioId]) ON DELETE CASCADE
);

CREATE TABLE [Doador] (
    [UsuarioId] int NOT NULL,
    [Cpf] nvarchar(max) NOT NULL,
    [DataNascimento] datetime2 NOT NULL,
    CONSTRAINT [PK_Doador] PRIMARY KEY ([UsuarioId]),
    CONSTRAINT [FK_Doador_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([UsuarioId]) ON DELETE CASCADE
);

CREATE TABLE [Ong] (
    [UsuarioId] int NOT NULL,
    [Cnpj] nvarchar(max) NOT NULL,
    [RazaoSocial] nvarchar(max) NOT NULL,
    [NomeFantasia] nvarchar(max) NOT NULL,
    [DataFundacao] datetime2 NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Ong] PRIMARY KEY ([UsuarioId]),
    CONSTRAINT [FK_Ong_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([UsuarioId]) ON DELETE CASCADE
);

CREATE TABLE [Voluntario] (
    [UsuarioId] int NOT NULL,
    [Cpf] nvarchar(max) NOT NULL,
    [DataNascimento] datetime2 NOT NULL,
    [Profissao] nvarchar(max) NOT NULL,
    [Disponibilidade] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Voluntario] PRIMARY KEY ([UsuarioId]),
    CONSTRAINT [FK_Voluntario_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([UsuarioId]) ON DELETE CASCADE
);

CREATE TABLE [Doacao] (
    [DoacaoId] int NOT NULL IDENTITY,
    [DoadorId] int NOT NULL,
    [OngId] int NOT NULL,
    [Categoria] nvarchar(max) NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Doacao] PRIMARY KEY ([DoacaoId]),
    CONSTRAINT [FK_Doacao_Doador_DoadorId] FOREIGN KEY ([DoadorId]) REFERENCES [Doador] ([UsuarioId]),
    CONSTRAINT [FK_Doacao_Ong_OngId] FOREIGN KEY ([OngId]) REFERENCES [Ong] ([UsuarioId])
);

CREATE TABLE [Evento] (
    [EventoId] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    [Data] datetime2 NOT NULL,
    [Localizacao] nvarchar(max) NOT NULL,
    [OngId] int NOT NULL,
    CONSTRAINT [PK_Evento] PRIMARY KEY ([EventoId]),
    CONSTRAINT [FK_Evento_Ong_OngId] FOREIGN KEY ([OngId]) REFERENCES [Ong] ([UsuarioId])
);

CREATE TABLE [PedidoDoacao] (
    [PedidoDoacaoId] int NOT NULL IDENTITY,
    [OngId] int NOT NULL,
    [Categoria] nvarchar(max) NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PedidoDoacao] PRIMARY KEY ([PedidoDoacaoId]),
    CONSTRAINT [FK_PedidoDoacao_Ong_OngId] FOREIGN KEY ([OngId]) REFERENCES [Ong] ([UsuarioId]) ON DELETE CASCADE
);

CREATE TABLE [VoluntarioEvento] (
    [VoluntarioId] int NOT NULL,
    [EventoId] int NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [DataInscricao] datetime2 NOT NULL,
    CONSTRAINT [PK_VoluntarioEvento] PRIMARY KEY ([VoluntarioId], [EventoId]),
    CONSTRAINT [FK_VoluntarioEvento_Evento_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [Evento] ([EventoId]),
    CONSTRAINT [FK_VoluntarioEvento_Voluntario_VoluntarioId] FOREIGN KEY ([VoluntarioId]) REFERENCES [Voluntario] ([UsuarioId])
);

CREATE INDEX [IX_Contato_UsuarioId] ON [Contato] ([UsuarioId]);

CREATE INDEX [IX_Doacao_DoadorId] ON [Doacao] ([DoadorId]);

CREATE INDEX [IX_Doacao_OngId] ON [Doacao] ([OngId]);

CREATE INDEX [IX_Evento_OngId] ON [Evento] ([OngId]);

CREATE INDEX [IX_PedidoDoacao_OngId] ON [PedidoDoacao] ([OngId]);

CREATE INDEX [IX_VoluntarioEvento_EventoId] ON [VoluntarioEvento] ([EventoId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250429194950_inicial-2025-04-29', N'9.0.4');

ALTER TABLE [PedidoDoacao] ADD [Status] nvarchar(max) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250429200507_status-pedidoDoacao-2025-04-29', N'9.0.4');

COMMIT;
GO


