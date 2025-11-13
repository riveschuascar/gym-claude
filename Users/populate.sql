INSERT INTO users.user (name, FirstLastname, SecondLastname, DateOfBirth, Ci, UserRole, HireDate, MonthlySalary, Specialization, Password, Email, MustChangePassword)
VALUES
('Juan', 'Perez', 'Quispe', '2000-07-14', '12345678', 'Instructor', '2025-09-30', 2000, 'Body Combat', crypt('instructor', gen_salt('bf')), 'instructor@gmail.com', true),
('Elad', 'Minist', 'Trador', '2000-12-24', '99999999', 'Admin', '2025-09-30', 2500, 'Administrar Sistema', crypt('admin', gen_salt('bf')), 'admin@gmail.com', true);
