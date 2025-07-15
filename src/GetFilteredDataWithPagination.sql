USE [Savr]
GO
/****** Object:  StoredProcedure [dbo].[GetFilteredDataWithPagination]    Script Date: 11/26/2024 5:18:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER   PROCEDURE [dbo].[GetFilteredDataWithPagination]
    @PageNumber INT,
    @PageSize INT,
    @NameFilter NVARCHAR(100) = NULL,
    @ManufactureEmailFilter NVARCHAR(100) = NULL,
    @PhoneFilter NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Calculate the starting row number
    DECLARE @StartRow INT;
    SET @StartRow = (@PageNumber - 1) * @PageSize + 1;

    -- Select filtered data with pagination
    WITH FilteredData AS
    (
        SELECT *,
            ROW_NUMBER() OVER (ORDER BY p.ProductDate) AS RowNum
        FROM dbo.Products as p
        WHERE (@NameFilter IS NULL OR p.Name LIKE '%' + @NameFilter + '%')
          AND (@ManufactureEmailFilter IS NULL OR p.ManufactureEmail LIKE '%' + @ManufactureEmailFilter + '%')
          AND (@PhoneFilter IS NULL OR p.ManufacturePhone LIKE '%' + @PhoneFilter + '%')
    )
    SELECT *
    FROM FilteredData
    WHERE RowNum BETWEEN @StartRow AND (@StartRow + @PageSize - 1);
END

