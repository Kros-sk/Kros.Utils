IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '{{TableName}}' AND type = 'U')
BEGIN

CREATE TABLE [{{TableName}}] (
	[TableName] nvarchar(100) NOT NULL,
	[LastId] {{DataType}} NOT NULL,

	CONSTRAINT [PK_{{TableName}}] PRIMARY KEY CLUSTERED ([TableName] ASC) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	)
)

END
