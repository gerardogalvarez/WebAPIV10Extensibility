EXEC STD_DropView 'V_Funcionarios'
GO

CREATE VIEW [dbo].[V_Funcionarios]
AS

	SELECT	*
	FROM	[Funcionarios] (NOLOCK)
GO

EXEC STD_DropView 'V_Faltas'
GO

CREATE VIEW [dbo].[V_Faltas]
AS

	SELECT	*
	FROM	[Faltas] (NOLOCK)
GO

EXEC STD_DropView 'V_Remuneracoes'
GO

CREATE VIEW [dbo].[V_Remuneracoes]
AS

	SELECT	*
	FROM	[Remuneracoes] (NOLOCK)
GO

EXEC STD_DropView 'V_Ausencias'
GO

CREATE VIEW [dbo].[V_Ausencias]
AS

	SELECT	*
	FROM	[TiposAusencia] (NOLOCK)
GO