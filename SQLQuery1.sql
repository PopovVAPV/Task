CREATE DATABASE HotelManagementSystem;
GO

USE HotelManagementSystem;
GO

-- ������� ����� �������
CREATE TABLE RoomTypes (
    room_type_id INT PRIMARY KEY IDENTITY(1,1),
    type_name VARCHAR(50) NOT NULL,
    description TEXT,
    base_price DECIMAL(10, 2) NOT NULL
);

-- ������� �������
CREATE TABLE Rooms (
    room_id INT PRIMARY KEY IDENTITY(1,1),
    room_number VARCHAR(10) NOT NULL UNIQUE,
    room_type_id INT NOT NULL,
    floor INT NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT '��������' 
        CHECK (status IN ('��������', '�����', '�������', '�� ������')),
    capacity INT NOT NULL,
    price_per_night DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (room_type_id) REFERENCES RoomTypes(room_type_id)
);

-- ������� ������
CREATE TABLE Guests (
    guest_id INT PRIMARY KEY IDENTITY(1,1),
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    phone VARCHAR(20) NOT NULL,
    email VARCHAR(100),
    passport_number VARCHAR(50) NOT NULL UNIQUE,
    preferences TEXT
);

-- ������� �����������
CREATE TABLE Staff (
    staff_id INT PRIMARY KEY IDENTITY(1,1),
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    position VARCHAR(20) NOT NULL 
        CHECK (position IN ('�������������', '������������', '�������')),
    email VARCHAR(100) UNIQUE,
    phone VARCHAR(20),
    hire_date DATE NOT NULL,
    login VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL
);

-- ������� ������������
CREATE TABLE Bookings (
    booking_id INT PRIMARY KEY IDENTITY(1,1),
    guest_id INT NOT NULL,
    room_id INT NOT NULL,
    check_in_date DATE NOT NULL,
    check_out_date DATE NOT NULL,
    booking_date DATETIME DEFAULT GETDATE(),
    status VARCHAR(20) DEFAULT '������������' 
        CHECK (status IN ('������������', '��������', '���������')),
    total_price DECIMAL(10, 2) NOT NULL,
    payment_status VARCHAR(20) DEFAULT '�� ��������' 
        CHECK (payment_status IN ('��������', '�� ��������', '�������� ��������')),
    FOREIGN KEY (guest_id) REFERENCES Guests(guest_id),
    FOREIGN KEY (room_id) REFERENCES Rooms(room_id),
    CHECK (check_out_date > check_in_date)
);

-- ������� ���� �������
CREATE TABLE KeyCards (
    card_id INT PRIMARY KEY IDENTITY(1,1),
    card_number VARCHAR(20) NOT NULL UNIQUE,
    issue_date DATETIME NOT NULL DEFAULT GETDATE(),
    expiration_date DATETIME,
    status VARCHAR(20) DEFAULT '�������' 
        CHECK (status IN ('�������', '�������������', '�������'))
);

-- ������� ���������
CREATE TABLE CheckIns (
    check_in_id INT PRIMARY KEY IDENTITY(1,1),
    booking_id INT NOT NULL UNIQUE,
    check_in_time DATETIME NOT NULL,
    check_out_time DATETIME,
    admin_id INT NOT NULL,
    card_id INT NOT NULL,
    FOREIGN KEY (booking_id) REFERENCES Bookings(booking_id),
    FOREIGN KEY (admin_id) REFERENCES Staff(staff_id),
    FOREIGN KEY (card_id) REFERENCES KeyCards(card_id)
);

-- ������� ��������
CREATE TABLE Payments (
    payment_id INT PRIMARY KEY IDENTITY(1,1),
    booking_id INT NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    payment_date DATETIME DEFAULT GETDATE(),
    payment_method VARCHAR(20) NOT NULL 
        CHECK (payment_method IN ('��������', '�����', '�������')),
    transaction_id VARCHAR(100),
    FOREIGN KEY (booking_id) REFERENCES Bookings(booking_id)
);

-- ������� ������
CREATE TABLE Cleaning (
    cleaning_id INT PRIMARY KEY IDENTITY(1,1),
    room_id INT NOT NULL,
    staff_id INT NOT NULL,
    scheduled_time DATETIME NOT NULL,
    completion_time DATETIME,
    status VARCHAR(20) DEFAULT '�������������' 
        CHECK (status IN ('�������������', '� ��������', '���������')),
    FOREIGN KEY (room_id) REFERENCES Rooms(room_id),
    FOREIGN KEY (staff_id) REFERENCES Staff(staff_id)
);