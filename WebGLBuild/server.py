import http.server
import socketserver

PORT = 8000

Handler = http.server.SimpleHTTPRequestHandler
# Явно указываем правильные типы для WebGL

with socketserver.TCPServer(("", PORT), Handler) as httpd:
    print(f"Сервер запущен на порту {PORT}")
    httpd.serve_forever()
