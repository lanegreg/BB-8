USE [VehicleContent]
GO

/****** Object:  StoredProcedure [CCWeb].[UrlRewriteLogRequest]    Script Date: 04/09/2015 11:07:00 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CCWeb].[UrlRewriteLogRequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [CCWeb].[UrlRewriteLogRequest]
GO

USE [VehicleContent]
GO

/****** Object:  StoredProcedure [CCWeb].[UrlRewriteLogRequest]    Script Date: 04/09/2015 11:07:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [CCWeb].[UrlRewriteLogRequest]
	@storedproc varchar(150),
	@input varchar(2000),
	@output varchar(2000),
	@instructioncode smallint,
	@input_hostname varchar(255) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
	INSERT INTO ccweb.UrlRewriteLog(sp_name, input_param, output_param, instruction_code, input_hostname)
	Values(@storedproc, @input, @output, @instructioncode, @input_hostname)
END

GO

