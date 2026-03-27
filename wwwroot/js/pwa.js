/* ============================================================
   pwa.js — PWA Registration + Install Prompt + Bottom Nav
   ============================================================ */

/* ── 1. Register Service Worker ── */
if ('serviceWorker' in navigator) {
  window.addEventListener('load', () => {
    navigator.serviceWorker
      .register('/sw.js')
      .then(reg => console.log('[PWA] SW registered, scope:', reg.scope))
      .catch(err => console.warn('[PWA] SW registration failed:', err));
  });
}

/* ── 2. PWA Install Prompt ── */
let deferredInstallPrompt = null;

window.addEventListener('beforeinstallprompt', e => {
  e.preventDefault();
  deferredInstallPrompt = e;

  if (!localStorage.getItem('pwa-dismissed')) {
    const banner = document.getElementById('pwa-install-banner');
    if (banner) banner.classList.add('show');
  }
});

document.addEventListener('DOMContentLoaded', () => {

  /* Install button */
  const installBtn = document.getElementById('pwa-install-btn');
  if (installBtn) {
    installBtn.addEventListener('click', async () => {
      if (!deferredInstallPrompt) return;
      deferredInstallPrompt.prompt();
      const { outcome } = await deferredInstallPrompt.userChoice;
      console.log('[PWA] Install outcome:', outcome);
      deferredInstallPrompt = null;
      hideBanner();
    });
  }

  /* Dismiss button */
  const dismissBtn = document.getElementById('pwa-dismiss-btn');
  if (dismissBtn) {
    dismissBtn.addEventListener('click', () => {
      localStorage.setItem('pwa-dismissed', '1');
      hideBanner();
    });
  }

  function hideBanner() {
    const banner = document.getElementById('pwa-install-banner');
    if (banner) banner.classList.remove('show');
  }

  /* ── 3. Bottom Nav Active State ── */

  // Build two segments from the current path:
  //   '/Dashboard/YearlyComparison' → seg1='/dashboard'  seg12='/dashboard/yearlycomparison'
  //   '/Expenses/Create'             → seg1='/expenses'   seg12='/expenses/create'
  //   '/Dashboard'                   → seg1='/dashboard'  seg12='/dashboard'   (same)
  //   '/'                            → seg1='/'           seg12='/'
  const parts = window.location.pathname.toLowerCase().split('/').filter(Boolean);
  const seg1  = '/' + (parts[0] || '');
  const seg12 = parts[1] ? seg1 + '/' + parts[1] : seg1;

  // Routes listed here are exact two-segment matches.
  // For sub-pages (/Expenses/Create, /Categories/Edit/2 etc.) the logic
  // falls back to seg1 so the parent nav item is still highlighted.
  // Yearly MUST be listed as its full two-segment path to separate it
  // from the Dashboard entry that shares the same first segment.
  const navMap = {
    'bnav-dashboard':  ['/dashboard', '/dashboard/index', '/dashboard/editsalary', '/'],
    'bnav-expenses':   ['/expenses'],
    'bnav-categories': ['/categories'],
    'bnav-yearly':     ['/dashboard/yearlycomparison'],
  };

  Object.entries(navMap).forEach(([id, routes]) => {
    const el = document.getElementById(id);
    if (!el) return;

    // Check the full two-segment path first (catches /dashboard/yearlycomparison),
    // then fall back to the first segment (catches /expenses/create → /expenses).
    const isActive = routes.includes(seg12) || routes.includes(seg1);
    if (isActive) {
      el.classList.add('active');
    }
  });

  /* ── 4. Standalone (installed) detection ── */
  if (window.matchMedia('(display-mode: standalone)').matches ||
      window.navigator.standalone === true) {
    document.body.classList.add('pwa-standalone');
    localStorage.setItem('pwa-dismissed', '1');
  }

});

/* ── 5. App installed event ── */
window.addEventListener('appinstalled', () => {
  console.log('[PWA] App installed successfully!');
  deferredInstallPrompt = null;
});
