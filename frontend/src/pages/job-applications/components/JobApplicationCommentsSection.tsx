import type { components } from "@/libs/api.schema.g";
import { formatDateTime } from "@/libs/utils";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";

type CommentDto = components["schemas"]["GetJobApplicationByIdCommentDto"];

interface JobApplicationCommentsSectionProps {
  comments: CommentDto[];
  newComment: string;
  isCommentSubmitting: boolean;
  isReadOnly?: boolean;
  onNewCommentChange: (value: string) => void;
  onAddComment: () => void;
  onDeleteComment: (id: string) => void;
}

export function JobApplicationCommentsSection({
  comments,
  newComment,
  isCommentSubmitting,
  isReadOnly = false,
  onNewCommentChange,
  onAddComment,
  onDeleteComment,
}: JobApplicationCommentsSectionProps) {
  return (
    <div className="space-y-3">
      <h3 className="text-base font-semibold">Comments</h3>

      {!isReadOnly ? (
        <div className="space-y-2">
          <Textarea
            value={newComment}
            onChange={(event) => onNewCommentChange(event.target.value)}
            placeholder="Add a comment"
            rows={3}
          />
          <Button onClick={onAddComment} disabled={isCommentSubmitting || !newComment.trim()}>
            {isCommentSubmitting ? "Adding..." : "Add Comment"}
          </Button>
        </div>
      ) : null}

      {comments.length === 0 ? (
        <p className="text-sm text-muted-foreground">No comments yet.</p>
      ) : (
        <div className="space-y-2">
          {comments.map((comment) => (
            <div key={comment.id} className="rounded-md border p-3">
              <p className="text-sm">{comment.content}</p>
              <div className="mt-2 flex items-center justify-between text-xs text-muted-foreground">
                <span>{formatDateTime(comment.createdAt)}</span>
                {!isReadOnly ? (
                  <Button
                    type="button"
                    variant="destructive"
                    className="h-7 px-2 text-xs"
                    onClick={() => onDeleteComment(comment.id)}
                  >
                    Delete
                  </Button>
                ) : null}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
