import { createProxyMiddleware } from 'http-proxy-middleware';

export default function (app: any) {
    app.use(
        '/_framework',
        createProxyMiddleware({
            target: 'http://localhost:5001', // Replace with your Blazor WebAssembly server URL
            changeOrigin: true,
            logLevel: 'debug',
        })
    );

    app.use(
        '/api',
        createProxyMiddleware({
            target: 'http://localhost:5002', // Replace with your API server URL
            changeOrigin: true,
            logLevel: 'debug',
        })
    );
}