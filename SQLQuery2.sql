SELECT 
    CAST(COUNT(CASE WHEN r.status = 'занят' THEN 1 END) AS FLOAT) / 
    COUNT(*) * 100 AS ПроцентЗагрузки
FROM Rooms r
LEFT JOIN Bookings b ON r.room_id = b.room_id
    AND b.status IN ('подтверждено', 'выполнено')
    AND '2025-04-30' >= b.check_in_date
    AND ('2025-04-30' < b.check_out_date OR b.check_out_date IS NULL);