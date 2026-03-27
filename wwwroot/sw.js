/* ============================================================
   SERVICE WORKER — Expense Tracker PWA
   Strategy: Cache-First for assets, Network-First for pages
   ============================================================ */

const CACHE_NAME   = 'expense-tracker-v1';
const STATIC_CACHE = 'expense-tracker-static-v1';

/* Assets we cache immediately on install */
const PRECACHE_ASSETS = [
  '/',
  '/Dashboard',
  '/css/site.css',
  '/js/site.js',
  '/js/pwa.js',
  '/icons/icon-192.png',
  '/icons/icon-512.png',
  /* CDN assets cached at runtime (see fetch handler) */
];

/* CDN origins we cache at runtime */
const CDN_ORIGINS = [
  'cdn.jsdelivr.net',
  'cdnjs.cloudflare.com',
  'code.jquery.com',
  'fonts.googleapis.com',
  'fonts.gstatic.com',
];

/* ── Install: pre-cache static assets ── */
self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(STATIC_CACHE).then(cache => {
      return cache.addAll(PRECACHE_ASSETS).catch(err => {
        /* Non-fatal: some assets may not exist yet (icons) */
        console.warn('[SW] Pre-cache partial failure:', err);
      });
    })
  );
  /* Skip waiting so the new SW activates immediately */
  self.skipWaiting();
});

/* ── Activate: clean up old caches ── */
self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(keys =>
      Promise.all(
        keys
          .filter(k => k !== CACHE_NAME && k !== STATIC_CACHE)
          .map(k => caches.delete(k))
      )
    )
  );
  /* Claim all clients so we control pages immediately */
  self.clients.claim();
});

/* ── Fetch: choose strategy by request type ── */
self.addEventListener('fetch', event => {
  const { request } = event;
  const url = new URL(request.url);

  /* Skip non-GET and cross-origin non-CDN requests */
  if (request.method !== 'GET') return;
  if (url.origin !== self.location.origin && !isCdnOrigin(url.hostname)) return;

  /* Strategy 1: Cache-First for CDN + static assets */
  if (isCdnOrigin(url.hostname) || isStaticAsset(url.pathname)) {
    event.respondWith(cacheFirst(request));
    return;
  }

  /* Strategy 2: Network-First for HTML pages (always fresh data) */
  if (request.headers.get('Accept')?.includes('text/html')) {
    event.respondWith(networkFirst(request));
    return;
  }

  /* Default: Cache-First */
  event.respondWith(cacheFirst(request));
});

/* ── Strategies ── */

async function cacheFirst(request) {
  const cached = await caches.match(request);
  if (cached) return cached;

  try {
    const response = await fetch(request);
    if (response.ok) {
      const cache = await caches.open(CACHE_NAME);
      cache.put(request, response.clone());
    }
    return response;
  } catch {
    return caches.match('/') || new Response('Offline', { status: 503 });
  }
}

async function networkFirst(request) {
  try {
    const response = await fetch(request);
    if (response.ok) {
      const cache = await caches.open(CACHE_NAME);
      cache.put(request, response.clone());
    }
    return response;
  } catch {
    const cached = await caches.match(request);
    return cached || caches.match('/') || new Response(offlinePage(), {
      headers: { 'Content-Type': 'text/html' }
    });
  }
}

/* ── Helpers ── */

function isCdnOrigin(hostname) {
  return CDN_ORIGINS.some(cdn => hostname.includes(cdn));
}

function isStaticAsset(pathname) {
  return /\.(css|js|woff2?|ttf|otf|png|jpg|jpeg|svg|ico|webp)$/i.test(pathname);
}

/* Minimal offline fallback page */
function offlinePage() {
  return `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8"/>
  <meta name="viewport" content="width=device-width,initial-scale=1"/>
  <title>Offline — Expense Tracker</title>
  <style>
    body{font-family:system-ui,sans-serif;display:flex;align-items:center;
         justify-content:center;min-height:100vh;margin:0;background:#f5f6fa;color:#1a1d2e;}
    .box{text-align:center;padding:2rem;}
    .icon{font-size:3rem;margin-bottom:1rem;}
    h1{font-size:1.4rem;margin:0 0 .5rem;}
    p{color:#6b7280;font-size:.9rem;}
    a{display:inline-block;margin-top:1.25rem;padding:.6rem 1.5rem;
      background:#4361ee;color:white;border-radius:100px;text-decoration:none;font-weight:600;}
  </style>
</head>
<body>
  <div class="box">
    <div class="icon">📡</div>
    <h1>You're offline</h1>
    <p>Check your connection and try again.<br/>Your data is safe.</p>
    <a href="/Dashboard">Retry</a>
  </div>
</body>
</html>`;
}
