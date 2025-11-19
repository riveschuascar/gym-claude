CREATE EXTENSION IF NOT EXISTS pgcrypto;

INSERT INTO users.user (name, FirstLastname, SecondLastname, DateOfBirth, Ci, UserRole, HireDate, MonthlySalary, Specialization, Password, Email, MustChangePassword)
VALUES
('Juan', 'Perez', 'Quispe', '2000-07-14', '12345678', 'Instructor', '2025-09-30', 2000, 'Body Combat', encode(digest('instructor', 'sha256'), 'hex'), 'instructor@gmail.com', true),
('Elad', 'Minist', 'Trador', '2000-12-24', '99999999', 'Admin', '2025-09-30', 2500, 'Administrar Sistema', encode(digest('admin', 'sha256'), 'hex'), 'admin@gmail.com', true),
('María', 'Condori', 'Mamani', '1995-03-22', '87654321', 'Instructor', '2025-10-15', 2100, 'Yoga', encode(digest('yoga2025', 'sha256'), 'hex'), 'maria.condori@gmail.com', true),
('Carlos', 'Ticona', 'Flores', '1998-11-08', '45678912', 'Instructor', '2025-10-01', 2000, 'CrossFit', encode(digest('crossfit123', 'sha256'), 'hex'), 'carlos.ticona@gmail.com', true),
('Ana', 'Vargas', 'Choque', '1992-05-17', '78945612', 'Instructor', '2025-09-25', 2200, 'Pilates', encode(digest('pilates456', 'sha256'), 'hex'), 'ana.vargas@gmail.com', true),
('Luis', 'Apaza', 'Huanca', '1997-08-30', '32165498', 'Instructor', '2025-10-20', 2050, 'Spinning', encode(digest('spinning789', 'sha256'), 'hex'), 'luis.apaza@gmail.com', true),
('Patricia', 'Gutierrez', 'Poma', '1993-01-12', '65498732', 'Instructor', '2025-09-28', 2150, 'Zumba', encode(digest('zumba321', 'sha256'), 'hex'), 'patricia.gutierrez@gmail.com', true),
('Roberto', 'Mamani', 'Quispe', '1996-06-25', '98765432', 'Instructor', '2025-10-10', 2100, 'Funcional', encode(digest('funcional654', 'sha256'), 'hex'), 'roberto.mamani@gmail.com', true),
('Sofia', 'Limachi', 'Chura', '1994-09-03', '15975348', 'Instructor', '2025-10-05', 2250, 'Body Pump', encode(digest('bodypump987', 'sha256'), 'hex'), 'sofia.limachi@gmail.com', true),
('Diego', 'Calle', 'Quisbert', '1999-02-18', '75395148', 'Instructor', '2025-10-12', 2000, 'Boxeo', encode(digest('boxeo147', 'sha256'), 'hex'), 'diego.calle@gmail.com', true),
('Gabriela', 'Nina', 'Condori', '1991-12-07', '85274196', 'Instructor', '2025-09-22', 2300, 'Natación', encode(digest('natacion258', 'sha256'), 'hex'), 'gabriela.nina@gmail.com', true),
('Fernando', 'Patty', 'Mamani', '1995-04-14', '95175386', 'Instructor', '2025-10-18', 2150, 'TRX', encode(digest('trx369', 'sha256'), 'hex'), 'fernando.patty@gmail.com', true),
('Valeria', 'Cruz', 'Lopez', '1998-07-29', '35795142', 'Recepcionista', '2025-10-08', 1800, 'Atención al Cliente', encode(digest('recepcion741', 'sha256'), 'hex'), 'valeria.cruz@gmail.com', true),
('Jorge', 'Rojas', 'Miranda', '1990-10-21', '25814736', 'Mantenimiento', '2025-09-20', 1900, 'Limpieza y Mantenimiento', encode(digest('mantenimiento852', 'sha256'), 'hex'), 'jorge.rojas@gmail.com', true),
('Daniela', 'Soria', 'Villarroel', '1996-03-05', '74185296', 'Nutricionista', '2025-10-22', 2400, 'Nutrición Deportiva', encode(digest('nutricion963', 'sha256'), 'hex'), 'daniela.soria@gmail.com', true);