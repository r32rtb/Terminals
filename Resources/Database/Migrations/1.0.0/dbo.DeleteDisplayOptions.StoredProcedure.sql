USE [{DATABASE_NAME}]
GO
/****** Object:  StoredProcedure [dbo].[DeleteDisplayOptions]    Script Date: 12/10/2012 22:16:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteDisplayOptions]
	(
	@FavoriteId int
	)
AS
	delete from DisplayOptions where FavoriteId = @FavoriteId
GO
