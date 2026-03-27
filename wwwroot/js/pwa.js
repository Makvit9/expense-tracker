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
  /* Prevent browser's default mini-infobar */
  e.preventDefault();
  deferredInstallPrompt = e;

  /* Only show banner if user hasn't dismissed it */
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
  const path = window.location.pathname.toLowerCase();

  const navMap = {
    'bnav-dashboard':   ['/dashboard', '/'],
    'bnav-expenses':    ['/expenses'],
    'bnav-categories':  ['/categories'],
    'bnav-yearly':      ['/yearlycomparison'],
  };

  Object.entries(navMap).forEach(([id, routes]) => {
    const el = document.getElementById(id);
    if (!el) return;
    if (routes.some(r => path.startsWith(r))) {
      el.classList.add('active');
    }
  });

  /* ── 4. Standalone (installed) detection ── */
  if (window.matchMedia('(display-mode: standalone)').matches ||
      window.navigator.standalone === true) {
    document.body.classList.add('pwa-standalone');
    /* In standalone mode, always hide the install banner */
    localStorage.setItem('pwa-dismissed', '1');
  }

});

/* ── 5. App installed event ── */
window.addEventListener('appinstalled', () => {
  console.log('[PWA] App installed successfully!');
  deferredInstallPrompt = null;
});
