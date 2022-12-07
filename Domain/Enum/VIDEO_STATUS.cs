using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum VIDEO_STATUS
    {
        CREATED,
        STARTED,
        CREATING_TITLE,
        CREATING_TEXT,
        CREATING_KEYWORDS,
        TRANSLATING_KEYWORDS,
        CREATING_IMAGES,
        CREATING_VOICE,
        CREATING_MUSIC,
        CREATING_VIDEO,
        READY_TO_EDIT,
        EDITING_VIDEO,
        READY_TO_UPLOAD,
        UPLOADING,
        FINISHED
    }
}
