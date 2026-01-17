USE [DB_SQL_PAIF_GAMES]
GO
delete from access_users
delete from access_control
delete from access_function
delete from access_profile

INSERT INTO dbo.access_profile (id_profile, name) VALUES (1, 'Administrador')
INSERT INTO dbo.access_profile (id_profile, name) VALUES (2, 'Cliente')

INSERT INTO dbo.access_function (id_function,name, code) VALUES (1, 'Incluir Usuário','CON-USU')
INSERT INTO dbo.access_function (id_function,name, code) VALUES (2, 'Listar Usuários','LIS-USU')
INSERT INTO dbo.access_function (id_function,name, code) VALUES (3, 'Alterar Usuário','ALT-USU')
INSERT INTO dbo.access_function (id_function,name, code) VALUES (4, 'Acessar Plataforma','ACE-PLA')
INSERT INTO dbo.access_function (id_function,name, code) VALUES (5, 'Acessar Biblioteca de Jogos','ACE-BIB-JOG')
INSERT INTO dbo.access_function (id_function,name, code) VALUES (6, 'Incluir/Alterar Jogos','INC-ALT-JOG')
INSERT INTO dbo.access_function (id_function,name, code) VALUES (7, 'Incluir/Alterar Promoção','INC-ALT-PRO')

INSERT INTO dbo.access_control VALUES (1,1)
INSERT INTO dbo.access_control VALUES (1,2)
INSERT INTO dbo.access_control VALUES (1,3)
INSERT INTO dbo.access_control VALUES (1,4)
INSERT INTO dbo.access_control VALUES (1,5)
INSERT INTO dbo.access_control VALUES (1,6)
INSERT INTO dbo.access_control VALUES (1,7)
INSERT INTO dbo.access_control VALUES (2,4)
INSERT INTO dbo.access_control VALUES (2,5)

INSERT INTO dbo.access_users (name, [login], [password], email, date_birth, id_profile) values ('CHARLES TOSTES', 'crtostes77', 'Paif@!01','crtostes77@gmail.com', '1977-01-06' ,1)


SELECT * FROM dbo.access_users u
	INNER JOIN dbo.access_profile p		ON u.id_profile = p.id_profile
	INNER JOIN dbo.access_control ac	ON p.id_profile = ac.id_profile
	INNER JOIN dbo.access_function f	ON ac.id_function = f.id_function
WHERE u.login = 'crtostes77'


INSERT INTO [dbo].[type_game] VALUES (1,'Ação')
INSERT INTO [dbo].[type_game] VALUES (2,'RPG(Role-Playng_Game')
INSERT INTO [dbo].[type_game] VALUES (3,'Estratégia')
INSERT INTO [dbo].[type_game] VALUES (4,'Simulação')
INSERT INTO [dbo].[type_game] VALUES (5,'Aventura')
INSERT INTO [dbo].[type_game] VALUES (6,'Esportes ou Fantasia')
INSERT INTO [dbo].[type_game] VALUES (7,'Battle Royale')


INSERT INTO [dbo].[promotion] ([id_promotion],[name],[discount],[date_start],[date_end]) VALUES (1,'Inauguração',10,'2026-01-01','2026-03-30')
INSERT INTO [dbo].[company] ([id_company],[name]) VALUES (1,'Atari')
INSERT INTO [dbo].[games] ([id_game],[name], id_company, id_type_game, price) VALUES (1,'River Raid',1,1,50.89)
INSERT INTO [dbo].[games] ([id_game],[name], id_company, id_type_game, price) VALUES (2,'Enduro',1,1,75.99)
INSERT INTO [dbo].[games] ([id_game],[name], id_company, id_type_game, price) VALUES (3,'Pole Position',1,1,85.99)
INSERT INTO [dbo].[games] ([id_game],[name], id_company, id_type_game, price) VALUES (4,'Pac-Man',1,1,24.89)


GO




GO


