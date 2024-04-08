INSERT INTO Players (FirstName, LastName, Nick, Elo, WinMatches, LostMatches, Draw, WinTournaments) VALUES
('Jan', 'Kowalski', 'Kowal', 1000, 0, 0, 0, 0),
('Mateusz', 'Nowak', 'Mat', 1000, 0, 0, 0, 0),
('Ola', 'Pietruszka', 'Ola', 1000, 0, 0, 0, 0),
('Piotr', 'Milik', 'Mil', 1000, 0, 0, 0, 0),
('Maciek', 'Klan', 'Klanik', 1000, 0, 0, 0, 0),
('Pawel', 'Gawel', 'Gawelek', 1000, 0, 0, 0, 0);

INSERT INTO Settings (StartElo, AllMatches, NumberOfMatchesPerPlayer, NumberOfSets, TieBreak, ExtraPointsForTournamentWon) VALUES
(1000, false, 3, 2, false, true);

INSERT INTO Tournaments (Name, Date, AllMatches, NumberOfMatchesPerPlayer, NumberOfSets, TieBreak, ExtraPointsForTournamentWon) VALUES
('Pierszy turniej', '2024-04-01', false, 3, 2, false, true),
('Drugi turniej', '2024-04-02', false, 2, 3, true, true);

INSERT INTO TournamentPlayers (TournamentId, PlayerId) VALUES
(1, 1),
(1, 2),
(1, 3),
(1, 4),
(1, 5),
(1, 6),
(2, 1),
(2, 2),
(2, 3),
(2, 4);

INSERT INTO Matches (TournamentId, MatchResult) VALUES
(1, 1),
(1, 1),
(1, 1),

(1, 1),
(1, 1),
(1, 1),

(1, 1),
(1, 1),
(1, 1);

INSERT INTO PlayerMatches (PlayerId, MatchId, Elo, Set1, Set2, Set3, Set4, Set5, TieBreak, WinnerResult) VALUES
(1, 1, 1000, 6, 6, null, null, null, null, 1),
(2, 1, 1000, 3, 4, null, null, null, null, 1),

(3, 2, 1000, 6, 6, null, null, null, null, 1),
(4, 2, 1000, 3, 4, null, null, null, null, 1),

(5, 3, 1000, 6, 6, null, null, null, null, 1),
(6, 3, 1000, 3, 4, null, null, null, null, 1),

(1, 4, 1000, 6, 6, null, null, null, null, 1),
(3, 4, 1000, 3, 4, null, null, null, null, 1),

(2, 5, 1000, 6, 6, null, null, null, null, 1),
(5, 5, 1000, 3, 4, null, null, null, null, 1),

(4, 6, 1000, 6, 6, null, null, null, null, 1),
(6, 6, 1000, 3, 4, null, null, null, null, 1),

(1, 7, 1000, 6, 6, null, null, null, null, 1),
(4, 7, 1000, 3, 4, null, null, null, null, 1),

(2, 8, 1000, 6, 6, null, null, null, null, 1),
(6, 8, 1000, 3, 4, null, null, null, null, 1),

(3, 9, 1000, 6, 6, null, null, null, null, 1),
(5, 9, 1000, 3, 4, null, null, null, null, 1);

INSERT INTO Matches (TournamentId, MatchResult) VALUES
(2, 1),
(2, 1),

(2, 1),
(2, 1);

INSERT INTO PlayerMatches (PlayerId, MatchId, Elo, Set1, Set2, Set3, Set4, Set5, TieBreak, WinnerResult) VALUES
(1, 10, 1000, 6, 6, null, null, null, null, 1),
(2, 10, 1000, 3, 4, null, null, null, null, 1),

(3, 11, 1000, 6, 6, null, null, null, null, 1),
(4, 11, 1000, 3, 4, null, null, null, null, 1),

(1, 12, 1000, 6, 6, null, null, null, null, 1),
(3, 12, 1000, 3, 4, null, null, null, null, 1),

(2, 13, 1000, 6, 6, null, null, null, null, 1),
(4, 13, 1000, 3, 4, null, null, null, null, 1);
