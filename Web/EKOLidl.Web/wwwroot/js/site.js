// „Накрая стигам до морето“ — progressive enhancements. Every page works without this file.
(() => {
  'use strict';

  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  // Scroll reveal: [data-reveal] or [data-reveal="2"] (the number staggers siblings).
  const revealItems = document.querySelectorAll('[data-reveal]');
  revealItems.forEach((el) => {
    const index = Number.parseInt(el.dataset.reveal, 10);
    if (!Number.isNaN(index)) el.style.setProperty('--reveal-index', String(index));
  });

  if ('IntersectionObserver' in window && !reduceMotion) {
    const revealObserver = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (!entry.isIntersecting) return;
        entry.target.classList.add('is-visible');
        revealObserver.unobserve(entry.target);
      });
    }, { rootMargin: '0px 0px -8% 0px', threshold: 0.08 });
    revealItems.forEach((el) => revealObserver.observe(el));
  } else {
    revealItems.forEach((el) => el.classList.add('is-visible'));
  }

  // Count-up numbers: <span data-count-to="600">600</span>. The server renders the final value.
  const counters = document.querySelectorAll('[data-count-to]');
  if ('IntersectionObserver' in window && !reduceMotion && counters.length) {
    const format = new Intl.NumberFormat('bg-BG');
    const countObserver = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (!entry.isIntersecting) return;
        countObserver.unobserve(entry.target);
        const el = entry.target;
        const target = Number.parseInt(el.dataset.countTo, 10);
        if (Number.isNaN(target)) return;
        const duration = 1400;
        const start = performance.now();
        const tick = (now) => {
          const t = Math.min((now - start) / duration, 1);
          const eased = 1 - Math.pow(1 - t, 4);
          el.textContent = format.format(Math.round(target * eased));
          if (t < 1) requestAnimationFrame(tick);
        };
        requestAnimationFrame(tick);
      });
    }, { threshold: 0.6 });
    counters.forEach((el) => countObserver.observe(el));
  }

  // Header: [data-header] gets data-scrolled once the page moves.
  const header = document.querySelector('[data-header]');
  if (header) {
    const update = () => header.toggleAttribute('data-scrolled', window.scrollY > 8);
    update();
    window.addEventListener('scroll', update, { passive: true });
  }

  // Mobile menu: a modal <dialog id="site-menu">.
  const menu = document.getElementById('site-menu');
  if (menu && typeof menu.showModal === 'function') {
    const toggles = document.querySelectorAll('[data-menu-open]');
    toggles.forEach((button) => button.addEventListener('click', () => {
      menu.showModal();
      button.setAttribute('aria-expanded', 'true');
    }));
    menu.addEventListener('close', () => toggles.forEach((b) => b.setAttribute('aria-expanded', 'false')));
    menu.querySelectorAll('[data-menu-close], a').forEach((el) => el.addEventListener('click', () => menu.close()));
    menu.addEventListener('click', (event) => {
      if (event.target === menu) menu.close();
    });
  }

  // Gallery lightbox: links marked [data-gallery-item] inside a [data-gallery] list.
  const galleries = document.querySelectorAll('[data-gallery]');
  if (galleries.length && typeof HTMLDialogElement === 'function') {
    const svg = (d) => `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">${d}</svg>`;
    const dialog = document.createElement('dialog');
    dialog.className = 'lightbox';
    dialog.setAttribute('aria-label', 'Преглед на снимка');
    dialog.innerHTML = `
      <figure class="lightbox-figure">
        <img class="lightbox-img" alt="">
        <figcaption class="lightbox-caption"></figcaption>
      </figure>
      <button type="button" class="lightbox-btn lightbox-close" data-lb="close" aria-label="Затвори">${svg('<path d="M18 6 6 18"/><path d="m6 6 12 12"/>')}</button>
      <button type="button" class="lightbox-btn lightbox-prev" data-lb="prev" aria-label="Предишна снимка">${svg('<path d="m15 18-6-6 6-6"/>')}</button>
      <button type="button" class="lightbox-btn lightbox-next" data-lb="next" aria-label="Следваща снимка">${svg('<path d="m9 18 6-6-6-6"/>')}</button>`;
    document.body.append(dialog);

    const image = dialog.querySelector('.lightbox-img');
    const caption = dialog.querySelector('.lightbox-caption');
    let items = [];
    let index = 0;
    let opener = null;

    const show = (i) => {
      index = (i + items.length) % items.length;
      const link = items[index];
      image.src = link.href;
      image.alt = link.dataset.caption || '';
      caption.textContent = link.dataset.caption || '';
      dialog.toggleAttribute('data-single', items.length < 2);
    };

    galleries.forEach((gallery) => {
      const links = [...gallery.querySelectorAll('[data-gallery-item]')];
      links.forEach((link, i) => link.addEventListener('click', (event) => {
        event.preventDefault();
        items = links;
        opener = link;
        show(i);
        dialog.showModal();
      }));
    });

    dialog.addEventListener('click', (event) => {
      const action = event.target.closest('[data-lb]')?.dataset.lb;
      if (action === 'close' || event.target === dialog) dialog.close();
      else if (action === 'prev') show(index - 1);
      else if (action === 'next') show(index + 1);
    });
    dialog.addEventListener('keydown', (event) => {
      if (event.key === 'ArrowLeft') show(index - 1);
      if (event.key === 'ArrowRight') show(index + 1);
    });
    dialog.addEventListener('close', () => opener?.focus());
  }

  // Copy buttons: <button data-copy="seaclub2@gmail.com" data-copied-label="Копирано">.
  document.querySelectorAll('[data-copy]').forEach((button) => {
    button.addEventListener('click', async () => {
      try {
        await navigator.clipboard.writeText(button.dataset.copy);
        const label = button.querySelector('[data-copy-label]');
        if (!label) return;
        const original = label.textContent;
        label.textContent = button.dataset.copiedLabel || 'Копирано';
        setTimeout(() => { label.textContent = original; }, 1800);
      } catch {
        // Clipboard unavailable (insecure context or permission denied): the text stays selectable.
      }
    });
  });
})();
