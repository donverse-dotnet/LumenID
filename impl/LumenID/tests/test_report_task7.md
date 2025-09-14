# How to test
1. Start mock database containers with compose.
2. Run backend and frontend project.
3. Access to register page and create new account.
4. Check successfully redirect to login page.
5. Login with registered account info.
6. Check successfully redirect to plain grant page.
7. Fix query param for grant page and reload.
8. Click grant button.
9. In frontend terminal, check successfully generated redirect for other apps.

# Problem
- All session information is shown in terminal.
- 3rd party application redirect is not working.
- In login/register page, does not have grant app query params.

# Total result
- [x]: Target (Grant code generate): is completed
