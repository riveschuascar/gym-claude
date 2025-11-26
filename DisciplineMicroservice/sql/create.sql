--BEGIN;

--DROP TABLE IF EXISTS public.discipline;

--CREATE TABLE public.discipline (
--    id                  SMALLSERIAL PRIMARY KEY,
--    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
--    last_modification   TIMESTAMPTZ,
--    is_active           BOOLEAN NOT NULL DEFAULT TRUE,

--    name                VARCHAR(20) NOT NULL,

--    id_user             BIGINT,

--    start_time          INTERVAL NOT NULL,
--    end_time            INTERVAL NOT NULL,

    
--    CONSTRAINT chk_discipline_name_chars CHECK (
--        name ~ '^[a-zA-Z0-9 ñáéíóúÁÉÍÓÚüÜ]+$'
--    ),

--    CONSTRAINT chk_discipline_letters CHECK (
--        length(regexp_replace(name, '[^A-Za-zñáéíóúÁÉÍÓÚüÜ]', '', 'g')) >= 3
--    ),

--    CONSTRAINT chk_discipline_digits CHECK (
--        length(regexp_replace(name, '[^0-9]', '', 'g')) <= 2
--    ),

--    CONSTRAINT chk_discipline_letters_if_digits CHECK (
--        length(regexp_replace(name, '[^0-9]', '', 'g')) = 0
--        OR length(regexp_replace(name, '[^A-Za-zñáéíóúÁÉÍÓÚüÜ]', '', 'g')) >= 5
--    ),

--    CONSTRAINT chk_discipline_time_order CHECK (
--        end_time > start_time
--    ),

--    CONSTRAINT chk_discipline_duration_min CHECK (
--        end_time - start_time >= INTERVAL '1 hour'
--    ),

--    CONSTRAINT chk_discipline_duration_max CHECK (
--        end_time - start_time <= INTERVAL '2 hour'
--    ),

--    CONSTRAINT chk_discipline_opening_hours CHECK (
--        start_time >= INTERVAL '06:00' AND 
--        end_time   <= INTERVAL '22:00'
--    )
--);

--CREATE INDEX idx_discipline_is_active ON public.discipline(is_active);
--CREATE INDEX idx_discipline_name      ON public.discipline(name);

--INSERT INTO public.discipline
--    (name, id_user, start_time, end_time)
--VALUES
--    ('Zumba',      1, '08:00', '09:30'),
--    ('Crossfit1',  2, '09:00', '10:30'),
--    ('YogaFit99',  3, '17:00', '18:00'),
--    ('Aerobicos',  4, '10:00', '11:00'),
--    ('BoxeoPro',   5, '18:00', '19:00'),
--    ('Pilates',    6, '15:00', '16:30');

--COMMIT;

-- Ajustar horario permitido para disciplinas a 06:00-22:00 en base existente
ALTER TABLE public.discipline
    DROP CONSTRAINT IF EXISTS chk_discipline_opening_hours;

ALTER TABLE public.discipline
    ADD CONSTRAINT chk_discipline_opening_hours
        CHECK (start_time >= INTERVAL '06:00' AND end_time <= INTERVAL '22:00');
