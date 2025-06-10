-- Добавление номеров
INSERT INTO public."Rooms" ("roomNumber", "roomType", availability, "roomCharge", capacity, area, description, "cleaningStatus", "lastCleaned", floor)
VALUES
-- Стандартные номера (roomType = 0)
(201, 0, 0, 2500.00, 2, 30.44, 'Стандартный номер с видом на город', 0, CURRENT_TIMESTAMP, 2),
(202, 0, 0, 2500.00, 2, 30.44, 'Стандартный номер с видом на парк', 0, CURRENT_TIMESTAMP, 2),
(203, 0, 0, 2500.00, 2, 30.44, 'Стандартный номер с видом на море', 0, CURRENT_TIMESTAMP, 2),

-- Стандарт плюс (roomType = 1)
(
    301,
    1,
    0,
    3500.00,
    2,
    35.00,
    'Улучшенный стандартный номер с мини-кухней',
    0,
    CURRENT_TIMESTAMP,
    3
),
(
    302,
    1,
    0,
    3500.00,
    2,
    35.00,
    'Улучшенный стандартный номер с балконом',
    0,
    CURRENT_TIMESTAMP,
    3
),

-- Семейные номера (roomType = 2)
(
    401,
    2,
    0,
    4500.00,
    4,
    45.00,
    'Семейный номер с двумя спальнями',
    0,
    CURRENT_TIMESTAMP,
    4
),
(
    402,
    2,
    0,
    4500.00,
    4,
    45.00,
    'Семейный номер с детской зоной',
    0,
    CURRENT_TIMESTAMP,
    4
),
(
    403,
    2,
    0,
    5000.00,
    5,
    50.00,
    'Большой семейный номер с гостиной',
    0,
    CURRENT_TIMESTAMP,
    4
),

-- Делюкс номера (roomType = 3)
(
    501,
    3,
    0,
    6000.00,
    2,
    55.00,
    'Делюкс номер с джакузи',
    0,
    CURRENT_TIMESTAMP,
    5
),
(
    502,
    3,
    0,
    6000.00,
    2,
    55.00,
    'Делюкс номер с панорамным видом',
    0,
    CURRENT_TIMESTAMP,
    5
),
(
    503,
    3,
    0,
    6500.00,
    3,
    60.00,
    'Делюкс номер с отдельной гостиной',
    0,
    CURRENT_TIMESTAMP,
    5
),

-- Люкс номера (roomType = 4)
(
    601,
    4,
    0,
    8000.00,
    2,
    70.00,
    'Люкс номер с террасой',
    0,
    CURRENT_TIMESTAMP,
    6
),
(
    602,
    4,
    0,
    8000.00,
    2,
    70.00,
    'Люкс номер с камином',
    0,
    CURRENT_TIMESTAMP,
    6
),
(
    603,
    4,
    0,
    9000.00,
    3,
    80.00,
    'Президентский люкс',
    0,
    CURRENT_TIMESTAMP,
    6
);

-- Добавление фотографий для номеров
INSERT INTO public."RoomPhotos" ("PhotoUrl", "IsPrimary", "UploadDate", "Description", "OrderIndex", "roomId")
SELECT 
    '/uploads/placeholder.jpg', -- Заглушка для URL фото
    true, -- Первое фото всегда основное
    CURRENT_TIMESTAMP,
    'Фото номера ' || r."roomNumber",
    0,
    r."roomId"
FROM public."Rooms" r
WHERE r."roomNumber" IN (201, 202, 203, 301, 302, 401, 402, 403, 501, 502, 503, 601, 602, 603);

-- Добавление вторых фотографий для некоторых номеров
INSERT INTO public."RoomPhotos" ("PhotoUrl", "IsPrimary", "UploadDate", "Description", "OrderIndex", "roomId")
SELECT 
    '/uploads/placeholder2.jpg', -- Заглушка для URL фото
    false,
    CURRENT_TIMESTAMP,
    'Дополнительное фото номера ' || r."roomNumber",
    1,
    r."roomId"
FROM public."Rooms" r
WHERE r."roomNumber" IN (401, 402, 403, 501, 502, 503, 601, 602, 603);