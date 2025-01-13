echo -e '\x1b[1;5;97;43m Open the site at http://localhost \x1b[0m'

(
    (cd Backend-Example/ && dotnet watch --quiet --project Presentation) &
    (cd react-app/ && pnpm start --host --logLevel error) &
    (cd admin-ui/ && pnpm start --host --port 3001 --logLevel error) &
    (./caddy.exe run) ;
kill 0)
