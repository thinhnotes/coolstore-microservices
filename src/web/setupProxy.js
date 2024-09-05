const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function(app) {
  app.use(
    '/_framework',
    createProxyMiddleware({
      target: 'https://localhost:5006',
      changeOrigin: true,
    })
  );

  app.use(
    '/content',
    createProxyMiddleware({
      target: 'https://localhost:5006',
      changeOrigin: true,
    })
  );
};
