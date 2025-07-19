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
            ROW_NUMBER() OVER (ORDER BY p.Listing) AS RowNum
        FROM dbo.Listing as p
        WHERE (@NameFilter IS NULL OR p.Name LIKE '%' + @NameFilter + '%')
          AND (@ManufactureEmailFilter IS NULL OR p.ManufactureEmail LIKE '%' + @ManufactureEmailFilter + '%')
          AND (@PhoneFilter IS NULL OR p.ManufacturePhone LIKE '%' + @PhoneFilter + '%')
    )
    SELECT *
    FROM FilteredData
    WHERE RowNum BETWEEN @StartRow AND (@StartRow + @PageSize - 1);
END

CREATE OR REPLACE FUNCTION get_dynamic_filtered_data_with_pagination(
    table_name TEXT,
    page_number INT,
    page_size INT,
    filter TEXT DEFAULT NULL,
    order_by TEXT DEFAULT 'id'
)
RETURNS SETOF RECORD
AS
$$
DECLARE
    sql TEXT;
    start_row INT := (page_number - 1) * page_size + 1;
    end_row INT := start_row + page_size - 1;
BEGIN
    IF table_name IS NULL OR TRIM(table_name) = '' THEN
        RAISE EXCEPTION 'TableName is required';
    END IF;

    sql := format(
        'SELECT * FROM (
            SELECT *, ROW_NUMBER() OVER (ORDER BY %s) AS rownum
            FROM %I
            %s
        ) sub
        WHERE rownum BETWEEN %s AND %s',
        order_by,
        table_name,
        CASE WHEN filter IS NOT NULL AND length(filter) > 0 THEN 'WHERE ' || filter ELSE '' END,
        start_row,
        end_row
    );

    RETURN QUERY EXECUTE sql;
END;
$$ LANGUAGE plpgsql;






-- Replace SETOF RECORD with concrete RETURN TABLE definition
CREATE OR REPLACE FUNCTION get_filtered_listings(
    page_number INT,
    page_size INT,
    filter TEXT DEFAULT NULL,
    order_by TEXT DEFAULT 'Id'
)
RETURNS TABLE (
    "Id" BIGINT,
    "Name" TEXT,
    "CreationDate" TIMESTAMP,
    "UpdateDate" TIMESTAMP,
    "Description" TEXT,
    "Location" TEXT,
    "AverageRating" NUMERIC,
    "IsAvailable" BOOLEAN,
    "UserId" UUID,
    "GroupId" BIGINT,
    "PreviousPrice" NUMERIC,
    "CurrentPrice" NUMERIC,
    "PriceWithPromotion" NUMERIC,
    "PriceDropPercentage" DOUBLE PRECISION
)
AS
$$
DECLARE
    sql TEXT;
BEGIN
    sql := format(
        'SELECT %s FROM (
            SELECT *, ROW_NUMBER() OVER (ORDER BY %I) AS rownum
            FROM "Listings"
            %s
        ) sub
        WHERE rownum BETWEEN %s AND %s',
        'Id, Name, CreationDate, UpdateDate, Description, Location, AverageRating, IsAvailable, UserId, GroupId, PreviousPrice, CurrentPrice, PriceWithPromotion, PriceDropPercentage',
        order_by,
        CASE WHEN filter IS NOT NULL AND length(filter) > 0 THEN 'WHERE ' || filter ELSE '' END,
        (page_number - 1) * page_size + 1,
        page_number * page_size
    );

    RETURN QUERY EXECUTE sql;
END;
$$ LANGUAGE plpgsql;



