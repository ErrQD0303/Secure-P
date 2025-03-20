SELECT 
    -- 'Secure Parking: ' || string_agg(relname, ', ') AS table_name
    concat('Secure Parking: ', string_agg(relname, ', ')) AS "Secure Parking Tables"
FROM 
    pg_class c
INNER JOIN
	pg_namespace n on c.relnamespace = n.oid
WHERE 
    relkind = 'r'  -- 'r' stands for regular tables, select tables only
    AND pg_catalog.pg_table_is_visible(c.oid)  -- To filter out internal or system tables
	AND n.nspname = 'public'
GROUP BY c.relnamespace;