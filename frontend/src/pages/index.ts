/**
 * Barrel file — re-exports all pages from their feature modules.
 * Routes (App.tsx) import ONLY from here. Never import pages directly from features/.
 */

export { default as Home } from '@/features/home/pages/Home/Home';
