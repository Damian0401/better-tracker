import type { ComponentProps } from "react";

interface SvgSpriteIconProps extends Omit<ComponentProps<"svg">, "children"> {
  iconId: string;
}

export function SvgSpriteIcon({ iconId, ...props }: SvgSpriteIconProps) {
  return (
    <svg {...props}>
      <use href={`/icons.svg#${iconId}`} />
    </svg>
  );
}
