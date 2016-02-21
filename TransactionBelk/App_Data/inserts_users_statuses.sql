INSERT INTO [dbo].[Users] (name, login, password, permission, isDeleted) values ('approver', 'approver', 'approver', 3, 0); 
INSERT INTO [dbo].[Users] (name, login, password, permission, isDeleted) values ('editor', 'editor', 'editor', 2, 0);
INSERT INTO [dbo].[Users] (name, login, password, permission, isDeleted) values ('viewer', 'viewer', 'viewer', 1, 0);
INSERT INTO [dbo].[Users] (name, login, password, permission, isDeleted) values ('root', 'root', 'root', 3, 0);


INSERT INTO [dbo].[TransactionStatus] (code, label) values (1, 'Pending');
INSERT INTO [dbo].[TransactionStatus] (code, label) values (1, 'Cancelled');
INSERT INTO [dbo].[TransactionStatus] (code, label) values (1, 'Approved');

