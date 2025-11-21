param(
    [string]$SessionId = "4766AFA9-D050-442E-839C-CE0497EBD556",
    [int]$TtlSeconds = 3600
)

$redisHost = "10.255.200.7"
$redisPort = 47846

Write-Host "This script helps you seed a session key into Redis for testing."
Write-Host "SessionId: $SessionId"
Write-Host "TTL (seconds): $TtlSeconds"

$redisCliCmd = "redis-cli -h $redisHost -p $redisPort SET sessionEntrevista:$SessionId \"exists\" EX $TtlSeconds"
$dockerCmd = "docker run --rm redis redis-cli -h $redisHost -p $redisPort SET sessionEntrevista:$SessionId \"exists\" EX $TtlSeconds"

Write-Host "If you have redis-cli installed, run:" -ForegroundColor Green
Write-Host $redisCliCmd

Write-Host "Or, using Docker (if installed), run:" -ForegroundColor Green
Write-Host $dockerCmd

Write-Host "Alternatively, use a GUI like AnotherRedisDesktopManager to create key: 'sessionEntrevista:$SessionId' with any value and TTL set to $TtlSeconds." -ForegroundColor Yellow

Write-Host "Done."
