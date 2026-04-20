const TAG_COLOR_CLASS_MAP = {
  rose: "border-rose-200 bg-rose-100 text-rose-800",
  amber: "border-amber-200 bg-amber-100 text-amber-800",
  emerald: "border-emerald-200 bg-emerald-100 text-emerald-800",
  sky: "border-sky-200 bg-sky-100 text-sky-800",
  indigo: "border-indigo-200 bg-indigo-100 text-indigo-800",
  cyan: "border-cyan-200 bg-cyan-100 text-cyan-800",
  lime: "border-lime-200 bg-lime-100 text-lime-800",
  teal: "border-teal-200 bg-teal-100 text-teal-800",
  orange: "border-orange-200 bg-orange-100 text-orange-800",
  fuchsia: "border-fuchsia-200 bg-fuchsia-100 text-fuchsia-800",
  violet: "border-violet-200 bg-violet-100 text-violet-800",
  blue: "border-blue-200 bg-blue-100 text-blue-800",
  slate: "border-slate-200 bg-slate-100 text-slate-800",
} as const;

const TAG_COLOR_KEYS = Object.keys(TAG_COLOR_CLASS_MAP) as Array<keyof typeof TAG_COLOR_CLASS_MAP>;

export const getTagColorClass = (tag: string): string => {
  const trimmedTag = tag.trim();
  if (!trimmedTag) {
    return TAG_COLOR_CLASS_MAP.slate;
  }

  const firstLetterCode = trimmedTag[0]?.toLowerCase().charCodeAt(0) ?? 0;
  const lastLetterCode = trimmedTag[trimmedTag.length - 1]?.toLowerCase().charCodeAt(0) ?? 0;
  const colorIndex = (firstLetterCode + lastLetterCode + trimmedTag.length) % TAG_COLOR_KEYS.length;
  const selectedKey = TAG_COLOR_KEYS[colorIndex] ?? "slate";

  return TAG_COLOR_CLASS_MAP[selectedKey];
};

export { TAG_COLOR_CLASS_MAP };
