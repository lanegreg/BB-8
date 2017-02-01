
/**!
 *	Table Name:		  PageTemplateMeta
 *	Lives In:		    VehicleContent
 *  Schema Name:    CCWeb
 *
 *	Author:			    Greg Lane
 *	Dependents:		  Carnado Site (CCWeb)
 *	Last Modified:	01/05/2015
 **/
 
 
USE VehicleContent
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

create table CCWeb.PageTemplateMeta (
	Id int identity(1,1) NOT NULL,
	UrlPathPattern varchar(100) NOT NULL,
	Metadata varchar(max) NOT NULL,
	CreatedOn datetime NOT NULL,
	constraint PK_PageTemplateMeta primary key clustered (
    Id ASC
  ) with (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
    on [PRIMARY]
) on [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

alter table CCWeb.PageTemplateMeta 
  add constraint DF_PageTemplateMeta_CreatedOn default (getdate()) 
  for CreatedOn
GO


/*
USE VehicleContent
GO

  drop table CCWeb.PageTemplateMeta

  select * from CCWeb.PageTemplateMeta

*/