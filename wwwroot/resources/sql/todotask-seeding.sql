INSERT INTO TodoTasks (
    Title, Type, DueDate, Note, ContactId,
    DeleteFlag, CreatedBy, CreatedAt,
    UpdatedBy, UpdatedAt, RecordVersion, IsCompleted
) VALUES
('Submit project proposal', 1, '2025-07-15', 'Submit to supervisor', 6, 0, NULL, NULL, NULL, NULL, NULL, 0),
('Weekly team meeting', 2, '2025-07-16', 'Discuss milestones', 6, 0, NULL, NULL, NULL, NULL, NULL, 0),
('Dentist appointment', 3, '2025-07-17', 'Routine checkup', 6, 0, NULL, NULL, NULL, NULL, NULL, 0),
('Buy groceries', 1, '2025-07-18', 'Milk, eggs, bread', 6, 0, NULL, NULL, NULL, NULL, NULL, 0),
('Read technical article', 2, '2025-07-19', 'On async/await in .NET', 6, 0, NULL, NULL, NULL, NULL, NULL, 0);